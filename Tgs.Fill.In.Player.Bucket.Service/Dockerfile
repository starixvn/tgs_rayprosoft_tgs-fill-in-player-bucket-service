#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Tgs.Fill.In.Player.Bucket.Service/Tgs.Fill.In.Player.Bucket.Service.csproj", "Tgs.Fill.In.Player.Bucket.Service/"]
RUN dotnet restore "Tgs.Fill.In.Player.Bucket.Service/Tgs.Fill.In.Player.Bucket.Service.csproj"
COPY . .
WORKDIR "/src/Tgs.Fill.In.Player.Bucket.Service"
RUN dotnet build "Tgs.Fill.In.Player.Bucket.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tgs.Fill.In.Player.Bucket.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Fill.In.Player.Bucket.Service.dll"]