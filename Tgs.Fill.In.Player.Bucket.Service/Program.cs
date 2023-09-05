using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.Fill.In.Player.Bucket.Core.DependencyInjection;
using Tgs.Fill.In.Player.Bucket.Redis.DependencyInjection;
using Tgs.Fill.In.Player.Bucket.Repositories.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
        .AddJsonFile("/config/appsettings.json", true)
        .AddEnvironmentVariables();

    builder.Services
        .AddCore()
        .AddRepositories((_, dbOptionsBuilder) => dbOptionsBuilder
            .UseNpgsql("name=Tgs"))
        .AddRedis((options, sp) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            options.ConnectionString = config.GetConnectionString("Redis")!;
        })
        .AddPlayerBucketTypeClient()
        .AddPlayerBucketClient(httpClientBuilder => httpClientBuilder
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress =
                builder.Configuration.GetValue<Uri>("FillInPlayerBucket:PlayerBucketBaseUrl")))
        .AddMemoryCache();

    builder.Services
        .AddMessageQueue()
        .AddNatsMessageQueue(natsConfig => natsConfig
            .ConfigQueueOptions((options, sp) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                options.Url = config.GetConnectionString("Nats")!;
            })
        )
        .AddNatsGlobPatternExchange("*");

    builder.Logging
        .AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.ParseStateValues = true;
            _ = options.AddFluentdForwardExporter(fluentOptions =>
            {
                var nodeIp = builder.Configuration.GetValue<string>("NODE_IP");
                fluentOptions.Host = nodeIp!;
                fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
                fluentOptions.UseMessagePack();
            });
        });

    builder.Services
        .AddOptions<OtlpExporterOptions>()
        .Configure<IConfiguration>((options, conifg) =>
        {
            var nodeIp = conifg.GetValue<string>("NODE_IP");

            if (!string.IsNullOrWhiteSpace(nodeIp))
            {
                options.Endpoint = new($"http://{nodeIp}:4317");
            }

            conifg.GetSection("Opentelemetry:Tracing").Bind(options);
        });

    builder.Services.AddOpenTelemetry()
        .WithTracing(builder => builder
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddEnvironmentVariableDetector())
            .AddSource("Tgs.*")
            .AddSource("TGS.*")
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
            .AddProfileViewExporter()
            .AddOtlpExporter())
        .Services
        .Configure<AspNetCoreInstrumentationOptions>(options => options.Filter =
            httpContext => !httpContext.Request.Path.HasValue
                || (!httpContext.Request.Path.StartsWithSegments("/swagger")
                    && !httpContext.Request.Path.StartsWithSegments("/healthz")
                    && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                    && !new[] { "html", "ico", "png", "jpg" }.Any(
                        ext => httpContext.Request.Path.Value.EndsWith($".{ext}"))));
    builder.Services.AddProfileViewer();

    builder.Services.AddHealthChecks();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
    app.UseSwaggerUI();
    app.MapControllers();
    app.MapSwagger();
    app.MapHealthChecks("/healthz");

    app.Run();
}