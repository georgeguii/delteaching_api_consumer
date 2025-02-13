FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["delteaching-dotnet/delteaching-dotnet.csproj", "delteaching-dotnet/"]
COPY ["del.shared/del.shared.csproj", "del.shared/"]
RUN dotnet restore "./delteaching-dotnet/delteaching-dotnet.csproj"
COPY . .
WORKDIR "/src/delteaching-dotnet"
RUN dotnet build "./delteaching-dotnet.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./delteaching-dotnet.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "delteaching-dotnet.dll"]
