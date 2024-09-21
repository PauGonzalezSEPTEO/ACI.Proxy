FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /work

COPY ./Directory.Build.props ./
COPY ./Directory.Packages.props ./
COPY ./ACI.HAM.ruleset ./
COPY src/ACI.HAM.Settings/*.csproj ./
COPY src/ACI.HAM.Api/*.csproj ./
COPY src/ACI.HAM.Core/*.csproj ./
COPY src/ACI.HAM.Mail/*.csproj ./
COPY src/ACI.HAM.Database/*.csproj ./
RUN for projectFile in $(ls *.csproj); \
  do \
    mkdir -p ${projectFile%.*}/ && mv $projectFile ${projectFile%.*}/; \
  done

ENV DOTNET_NOLOGO=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true

RUN dotnet restore /work/ACI.HAM.Api/ACI.HAM.Api.csproj

COPY src .

FROM build AS publish
WORKDIR /work/ACI.HAM.Api

ENV DOTNET_NOLOGO=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true

RUN dotnet publish -c Debug -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app .


ENV DOTNET_NOLOGO=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true

EXPOSE 80

ENTRYPOINT ["dotnet", "ACI.HAM.Api.dll"]
