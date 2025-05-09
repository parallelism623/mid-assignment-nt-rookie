# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MIDASM.API/MIDASM.API.csproj", "src/MIDASM.API/"]
COPY ["src/MIDASM.Application/MIDASM.Application.csproj", "src/MIDASM.Application/"]
COPY ["src/MIDASM.Contract/MIDASM.Contract.csproj", "src/MIDASM.Contract/"]
COPY ["src/MIDASM.Domain/MIDASM.Domain.csproj", "src/MIDASM.Domain/"]
COPY ["src/MIDASM.Infrastructure/MIDASM.Infrastructure.csproj", "src/MIDASM.Infrastructure/"]
COPY ["src/MIDASM.Persistence/MIDASM.Persistence.csproj", "src/MIDASM.Persistence/"]
RUN dotnet restore "./src/MIDASM.API/MIDASM.API.csproj"
COPY . .
WORKDIR "/src/src/MIDASM.API"
RUN dotnet build "./MIDASM.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MIDASM.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MIDASM.API.dll"]