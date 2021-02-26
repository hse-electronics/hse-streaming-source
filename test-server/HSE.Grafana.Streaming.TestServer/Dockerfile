#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["HSE.Grafana.Streaming.TestServer/HSE.Grafana.Streaming.TestServer.csproj", "HSE.Grafana.Streaming.TestServer/"]
RUN dotnet restore "HSE.Grafana.Streaming.TestServer/HSE.Grafana.Streaming.TestServer.csproj"
COPY . .
WORKDIR "/src/HSE.Grafana.Streaming.TestServer"
RUN dotnet build "HSE.Grafana.Streaming.TestServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HSE.Grafana.Streaming.TestServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HSE.Grafana.Streaming.TestServer.dll"]
