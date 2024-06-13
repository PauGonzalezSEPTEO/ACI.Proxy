FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /work

COPY ./Directory.Build.props ./
COPY ./Directory.Packages.props ./
COPY ./ACI.Base.ruleset ./
COPY src/ACI.Base.Settings/*.csproj ./
COPY src/ACI.Base.Api/*.csproj ./
COPY src/ACI.Base.Core/*.csproj ./
COPY src/ACI.Base.Mail/*.csproj ./
COPY src/ACI.Base.Database/*.csproj ./
RUN for projectFile in $(ls *.csproj); \
  do \
    mkdir -p ${projectFile%.*}/ && mv $projectFile ${projectFile%.*}/; \
  done

ENV DOTNET_NOLOGO=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true

RUN dotnet restore /work/ACI.Base.Api/ACI.Base.Api.csproj

COPY src .

FROM build AS publish
WORKDIR /work/ACI.Base.Api

ENV DOTNET_NOLOGO=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true

RUN dotnet publish -c Debug -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app .


ENV DOTNET_NOLOGO=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true

ENTRYPOINT ["dotnet", "ACI.Base.Api.dll"]
