# 🐳 Dockerfile Completo - Sistema Minha Empresa
# Multi-stage build otimizado para produção

# Stage 1: Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Criar usuário não-root para segurança
RUN adduser --disabled-password --gecos '' appuser && \
    mkdir -p /app/wwwroot /app/data-protection-keys /app/logs && \
    chown -R appuser:appuser /app
USER appuser

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências (cache layer)
COPY ["MinhaEmpresa.Web.csproj", "./"]
RUN dotnet restore "./MinhaEmpresa.Web.csproj"

# Copiar código fonte
COPY . .

# Build da aplicação
RUN dotnet build "MinhaEmpresa.Web.csproj" -c Release -o /app/build

# Stage 3: Publish
FROM build AS publish
RUN dotnet publish "MinhaEmpresa.Web.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

# Stage 4: Final
FROM base AS final
WORKDIR /app

# Copiar arquivos publicados
COPY --from=publish /app/publish .

# Criar diretório wwwroot se não existir (corrige warning)
RUN mkdir -p wwwroot

# Configurar variáveis de ambiente para produção
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_NOLOGO=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=true

# Configurar Data Protection para container (corrige warnings)
ENV ASPNETCORE_DataProtection__ApplicationName=MinhaEmpresaApp
VOLUME ["/app/data-protection-keys"]

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Comando de inicialização
ENTRYPOINT ["dotnet", "MinhaEmpresa.Web.dll"]
