#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["DockerDashboard.Api/DockerDashboard.Api.csproj", "DockerDashboard.Api/"]
COPY ["DockerDashboard.Infrastructure/DockerDashboard.Infrastructure.csproj", "DockerDashboard.Infrastructure/"]
RUN dotnet restore "DockerDashboard.Api/DockerDashboard.Api.csproj"
COPY . .
WORKDIR "/src/DockerDashboard.Api"
RUN dotnet build "DockerDashboard.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerDashboard.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerDashboard.Api.dll"]