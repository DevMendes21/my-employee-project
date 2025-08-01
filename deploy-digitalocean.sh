#!/bin/bash

# 🚀 Script de Deploy para DigitalOcean
# Automatiza o processo de deploy da aplicação Minha Empresa

set -e

echo "🚀 Iniciando processo de deploy na DigitalOcean..."
echo "======================================================"

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

log_success "Diretório do projeto verificado"

# 2. Verificar se o Git está limpo
if [ -n "$(git status --porcelain)" ]; then
    log_warning "Há alterações não commitadas no Git"
    read -p "Deseja continuar mesmo assim? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        log_info "Deploy cancelado pelo usuário"
        exit 1
    fi
fi

# 3. Testar build local
log_info "Testando build local..."
if dotnet build MinhaEmpresa.Web.csproj -c Release > /dev/null 2>&1; then
    log_success "Build local OK"
else
    log_error "Erro no build local"
    log_info "Execute: dotnet build MinhaEmpresa.Web.csproj -c Release"
    exit 1
fi

# 4. Testar Docker build (opcional)
read -p "Deseja testar o Docker build localmente? (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    log_info "Testando Docker build..."
    if docker build -t minha-empresa-test . > /dev/null 2>&1; then
        log_success "Docker build OK"
        docker rmi minha-empresa-test > /dev/null 2>&1
    else
        log_error "Erro no Docker build"
        exit 1
    fi
fi

# 5. Verificar arquivos de configuração sensíveis
log_warning "🔒 VERIFICANDO SEGURANÇA..."

# Verificar se dados sensíveis não estão commitados
if grep -r "corelink-db" . --exclude-dir=.git --exclude="*.sh" --exclude="*.md" 2>/dev/null; then
    log_error "DADOS SENSÍVEIS ENCONTRADOS NO CÓDIGO!"
    log_error "Remova todos os dados sensíveis antes do commit"
    exit 1
fi

# Verificar se .env existe
if [ ! -f ".env" ]; then
    log_warning "Arquivo .env não encontrado"
    log_info "Criando arquivo .env template..."
    cp .env.template .env
    log_error "IMPORTANTE: Edite o arquivo .env com suas credenciais reais!"
    log_error "Nunca commite o arquivo .env no Git!"
    exit 1
fi

# Verificar se app.yaml tem dados sensíveis
if [ -f ".do/app.yaml" ] && grep -q "corelink-db\|AVNS_" .do/app.yaml 2>/dev/null; then
    log_error "Dados sensíveis encontrados em .do/app.yaml!"
    log_info "Use .do/app.template.yaml e configure as variáveis na DigitalOcean"
    exit 1
fi

# 6. Commit e push das alterações
log_info "Preparando commit para deploy..."
git add .
git commit -m "🚀 Deploy: Configuração para DigitalOcean App Platform" || log_warning "Nada para commitar"

read -p "Fazer push para o repositório? (Y/n): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Nn]$ ]]; then
    log_info "Fazendo push para GitHub..."
    git push origin main
    log_success "Push realizado com sucesso"
fi

# 7. Instruções para DigitalOcean
echo ""
echo "======================================================"
log_success "Preparação para deploy concluída!"
echo "======================================================"
echo ""
log_info "📋 PRÓXIMOS PASSOS:"
echo ""
echo "1. 🌐 Acesse: https://cloud.digitalocean.com/apps"
echo ""
echo "2. 🔘 Clique em 'Create App'"
echo ""
echo "3. 📚 Selecione 'GitHub' como fonte"
echo ""
echo "4. 🏗️  Escolha o repositório: DevMendes21/my-employee-project"
echo ""
echo "5. 🌿 Branch: main"
echo ""
echo "6. ⚙️  Use o arquivo de configuração: .do/app.yaml"
echo ""
echo "7. 🚀 Clique em 'Next' e depois 'Create Resources'"
echo ""
echo "======================================================"
log_info "🔗 Sua aplicação estará disponível em:"
log_info "https://minha-empresa-system-xxxxx.ondigitalocean.app"
echo "======================================================"
echo ""
log_warning "⏱️  O deploy pode levar de 5-10 minutos"
log_info "📊 Monitore o progresso no painel da DigitalOcean"
echo ""
log_success "🎉 Deploy iniciado com sucesso!"
