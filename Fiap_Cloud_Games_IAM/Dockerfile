# Etapa 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . .

# Restaura e publica
RUN dotnet restore ./Fiap_Cloud_Games_IAM/Fiap_Cloud_Games_IAM.csproj
RUN dotnet publish ./Fiap_Cloud_Games_IAM/Fiap_Cloud_Games_IAM.csproj -c Release -o /app/publish

# Etapa 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Instala o agente New Relic via APT
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
  && echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' > /etc/apt/sources.list.d/newrelic.list \
  && wget -O - https://download.newrelic.com/548C16BF.gpg | apt-key add - \
  && apt-get update \
  && apt-get install -y newrelic-dotnet-agent \
  && rm -rf /var/lib/apt/lists/*

# Configuração do New Relic
ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER="{36032161-FFC0-4B61-B559-F6C5D41BAE5A}" \
    CORECLR_NEWRELIC_HOME="/usr/local/newrelic-dotnet-agent" \
    CORECLR_PROFILER_PATH="/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so" \
    NEW_RELIC_LICENSE_KEY="bb4d23e7af2579ee8619da7c33f5cab8FFFFNRAL"\
    NEW_RELIC_APP_NAME="fiap-cloud-games-iam" \
    NEW_RELIC_DISTRIBUTED_TRACING_ENABLED=true \
    NEW_RELIC_LOG_LEVEL=info \
    NEW_RELIC_APPLICATION_LOGGING_ENABLED=true \
    NEW_RELIC_APPLICATION_LOGGING_FORWARDING_ENABLED=true \
    NEW_RELIC_APPLICATION_LOGGING_FORWARDING_CONTEXT_DATA_ENABLED=true \
    NEW_RELIC_APPLICATION_LOGGING_FORWARDING_MAX_SAMPLES_STORED=10000 \
    NEW_RELIC_APPLICATION_LOGGING_LOCAL_DECORATING_ENABLED=false

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Fiap_Cloud_Games_IAM.dll"]
