#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.Fill.In.Player.Bucket.Service/Tgs.Fill.In.Player.Bucket.Service.csproj", "Tgs.Fill.In.Player.Bucket.Service/"]
COPY ["Tgs.Fill.In.Player.Bucket.Core/Tgs.Fill.In.Player.Bucket.Core.csproj", "Tgs.Fill.In.Player.Bucket.Core/"]
COPY ["Tgs.Fill.In.Player.Bucket.Domain/Tgs.Fill.In.Player.Bucket.Domain.csproj", "Tgs.Fill.In.Player.Bucket.Domain/"]
COPY ["Tgs.Fill.In.Player.Bucket.Redis/Tgs.Fill.In.Player.Bucket.Redis.csproj", "Tgs.Fill.In.Player.Bucket.Redis/"]
COPY ["Tgs.Fill.In.Player.Bucket.Repositories/Tgs.Fill.In.Player.Bucket.Repositories.csproj", "Tgs.Fill.In.Player.Bucket.Repositories/"]
RUN dotnet restore "Tgs.Fill.In.Player.Bucket.Service/Tgs.Fill.In.Player.Bucket.Service.csproj"
COPY . .
WORKDIR "/src/Tgs.Fill.In.Player.Bucket.Service"
RUN dotnet build "Tgs.Fill.In.Player.Bucket.Service.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.Fill.In.Player.Bucket.Service.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Fill.In.Player.Bucket.Service.dll"]