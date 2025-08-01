#!/bin/bash

# 🐳 Script para testar Docker build
# Ferramenta de debugging para problemas de containerização

set -e

echo "🐳 Testando Docker Build - Sistema Minha Empresa"
echo "================================================"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para logs coloridos
log_info() { echo -e "${BLUE}ℹ️  $1${NC}"; }
log_success() { echo -e "${GREEN}✅ $1${NC}"; }
log_warning() { echo -e "${YELLOW}⚠️  $1${NC}"; }
log_error() { echo -e "${RED}❌ $1${NC}"; }

# 1. Verificar se estamos no diretório correto
if [ ! -f "MinhaEmpresa.Web.csproj" ]; then
    log_error "Arquivo MinhaEmpresa.Web.csproj não encontrado!"
    log_error "Execute este script no diretório raiz do projeto."
    exit 1
fi

# 2. Verificar se Docker está instalado
if ! command -v docker &> /dev/null; then
    log_error "Docker não está instalado!"
    log_info "Para instalar o Docker:"
    log_info "1. Acesse: https://docker.com/get-started"
    log_info "2. Baixe Docker Desktop para Mac"
    log_info "3. Instale e reinicie o computador"
    exit 1
fi

log_success "Docker está instalado"

# 3. Verificar se Docker está rodando
log_info "Verificando se Docker daemon está rodando..."
if ! docker info > /dev/null 2>&1; then
    log_error "Docker daemon não está rodando!"
    log_info "Para iniciar o Docker:"
    log_info "1. Abra o Docker Desktop (ícone na barra de tarefas)"
    log_info "2. Aguarde aparecer 'Docker Desktop is running'"
    log_info "3. Execute este script novamente"
    
    # Tentar iniciar Docker automaticamente
    log_info "Tentando iniciar Docker automaticamente..."
    open -a Docker
    
    log_warning "Aguardando Docker inicializar (30 segundos)..."
    sleep 30
    
    if docker info > /dev/null 2>&1; then
        log_success "Docker iniciado com sucesso!"
    else
        log_error "Não foi possível iniciar Docker automaticamente"
        log_info "Inicie o Docker Desktop manualmente e tente novamente"
        exit 1
    fi
fi

log_success "Docker daemon está rodando"

# 4. Verificar informações do Docker
log_info "Informações do Docker:"
docker --version
echo ""

# 5. Verificar se Dockerfile existe
if [ ! -f "Dockerfile" ]; then
    log_error "Dockerfile não encontrado!"
    log_info "Criando Dockerfile básico..."
    
    cat > Dockerfile << 'EOF'
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MinhaEmpresa.Web.csproj", "."]
RUN dotnet restore "./MinhaEmpresa.Web.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "MinhaEmpresa.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MinhaEmpresa.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MinhaEmpresa.Web.dll"]
EOF
    
    log_success "Dockerfile criado"
fi

# 6. Limpar imagens antigas se existirem
log_info "Limpando imagens de teste antigas..."
docker rmi minha-empresa-test 2>/dev/null || true
docker rmi minha-empresa-debug 2>/dev/null || true

# 7. Executar build com output detalhado
log_info "Iniciando Docker build..."
echo "========================================"

if docker build -t minha-empresa-debug .; then
    log_success "🎉 Docker build concluído com sucesso!"
    
    # 8. Testar se a imagem foi criada corretamente
    log_info "Verificando imagem criada..."
    docker images minha-empresa-debug
    
    # 9. Testar execução rápida (opcional)
    read -p "Deseja testar a execução da imagem? (y/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        log_info "Testando execução da imagem (30 segundos)..."
        log_warning "A aplicação pode falhar por falta de variáveis de ambiente - isso é normal!"
        
        # Executar em background e matar após 30s
        docker run --name minha-empresa-test-run -p 8080:8080 minha-empresa-debug &
        CONTAINER_PID=$!
        
        sleep 5
        
        # Testar se a porta está respondendo
        if curl -s http://localhost:8080 > /dev/null 2>&1; then
            log_success "Aplicação respondendo na porta 8080!"
        else
            log_warning "Aplicação não está respondendo (pode ser falta de env vars)"
        fi
        
        # Parar container
        docker stop minha-empresa-test-run 2>/dev/null || true
        docker rm minha-empresa-test-run 2>/dev/null || true
    fi
    
    # 10. Limpar imagem de teste
    log_info "Limpando imagem de teste..."
    docker rmi minha-empresa-debug
    
    echo ""
    echo "========================================"
    log_success "✅ Docker build está funcionando!"
    log_info "Sua aplicação está pronta para deploy na DigitalOcean"
    echo "========================================"
    
else
    log_error "❌ Docker build falhou!"
    echo ""
    log_info "🔧 Possíveis soluções:"
    log_info "1. Verificar se todos os arquivos estão presentes"
    log_info "2. Verificar se o .NET 8 SDK está funcionando localmente"
    log_info "3. Verificar erros de sintaxe no Dockerfile"
    log_info "4. Tentar: docker system prune -f (limpar cache)"
    echo ""
    exit 1
fi
