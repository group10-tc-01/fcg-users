FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["src/FCG.Users.Application/FCG.Users.Application.csproj", "src/FCG.Users.Application/"]
COPY ["src/FCG.Users.Domain/FCG.Users.Domain.csproj", "src/FCG.Users.Domain/"]
COPY ["src/FCG.Users.Infrastructure.Auth/FCG.Users.Infrastructure.Auth.csproj", "src/FCG.Users.Infrastructure.Auth/"]
COPY ["src/FCG.Users.Infrastructure.Kafka/FCG.Users.Infrastructure.Kafka.csproj", "src/FCG.Users.Infrastructure.Kafka/"]
COPY ["src/FCG.Users.Infrastructure.SqlServer/FCG.Users.Infrastructure.SqlServer.csproj", "src/FCG.Users.Infrastructure.SqlServer/"]
COPY ["src/FCG.Users.Messages/FCG.Users.Messages.csproj", "src/FCG.Users.Messages/"]
COPY ["src/FCG.Users.WebApi/FCG.Users.WebApi.csproj", "src/FCG.Users.WebApi/"]
RUN dotnet restore "./src/FCG.Users.WebApi/FCG.Users.WebApi.csproj"
COPY . .
WORKDIR "/src/src/FCG.Users.WebApi"
RUN dotnet build "./FCG.Users.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FCG.Users.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG.Users.WebApi.dll"]
