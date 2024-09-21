# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ACI.HAM.Api/ACI.HAM.Api.csproj", "src/ACI.HAM.Api/"]
COPY ["src/ACI.HAM.Core/ACI.HAM.Core.csproj", "src/ACI.HAM.Core/"]
COPY ["src/ACI.HAM.Mail/ACI.HAM.Mail.csproj", "src/ACI.HAM.Mail/"]
COPY ["src/ACI.HAM.Settings/ACI.HAM.Settings.csproj", "src/ACI.HAM.Settings/"]
RUN dotnet restore "./src/ACI.HAM.Api/ACI.HAM.Api.csproj"
COPY . .
WORKDIR "/src/src/ACI.HAM.Api"
RUN dotnet build "./ACI.HAM.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ACI.HAM.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACI.HAM.Api.dll"]