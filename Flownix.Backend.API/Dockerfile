#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Flownix.Backend.API/Flownix.Backend.API.csproj", "Flownix.Backend.API/"]
COPY ["Flownix.Backend.Application/Flownix.Backend.Application.csproj", "Flownix.Backend.Application/"]
COPY ["Flownix.Backend.Contracts/Flownix.Backend.Contracts.csproj", "Flownix.Backend.Contracts/"]
COPY ["Flownix.Backend.Domain/Flownix.Backend.Domain.csproj", "Flownix.Backend.Domain/"]
COPY ["Flownix.Backend.Infrastructure/Flownix.Backend.Infrastructure.csproj", "Flownix.Backend.Infrastructure/"]
RUN dotnet restore "./Flownix.Backend.API/Flownix.Backend.API.csproj"
COPY . .
WORKDIR "/src/Flownix.Backend.API"
RUN dotnet build "./Flownix.Backend.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Flownix.Backend.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Flownix.Backend.API.dll"]