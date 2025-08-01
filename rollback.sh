#!/bin/bash

# 🔄 Script de Rollback para DigitalOcean
# Permite reverter para uma versão anterior da aplicação

set -e

echo "🔄 Script de Rollback - DigitalOcean App Platform"
echo "================================================"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log_info() { echo -e "${BLUE}ℹ️  $1${NC}"; }
log_success() { echo -e "${GREEN}✅ $1${NC}"; }
log_warning() { echo -e "${YELLOW}⚠️  $1${NC}"; }
log_error() { echo -e "${RED}❌ $1${NC}"; }

# Verificar se doctl está instalado
if ! command -v doctl &> /dev/null; then
    log_warning "DigitalOcean CLI (doctl) não está instalado"
    log_info "Instalando doctl..."
    
    if command -v brew &> /dev/null; then
        brew install doctl
    else
        log_error "Homebrew não encontrado. Instale o doctl manualmente:"
        log_info "https://docs.digitalocean.com/reference/doctl/how-to/install/"
        exit 1
    fi
fi

# Verificar autenticação
if ! doctl account get > /dev/null 2>&1; then
    log_warning "Não está autenticado na DigitalOcean"
    log_info "Execute: doctl auth init"
    exit 1
fi

# Listar apps
log_info "Listando aplicações..."
echo ""
doctl apps list

echo ""
read -p "Digite o ID da aplicação: " APP_ID

if [ -z "$APP_ID" ]; then
    log_error "ID da aplicação é obrigatório"
    exit 1
fi

# Listar deployments
log_info "Listando deployments da aplicação $APP_ID..."
echo ""
doctl apps list-deployments $APP_ID

echo ""
read -p "Digite o ID do deployment para rollback: " DEPLOYMENT_ID

if [ -z "$DEPLOYMENT_ID" ]; then
    log_error "ID do deployment é obrigatório"
    exit 1
fi

# Confirmar rollback
echo ""
log_warning "⚠️  ATENÇÃO: Você está prestes a fazer rollback para o deployment $DEPLOYMENT_ID"
read -p "Tem certeza? Esta ação não pode ser desfeita. (y/N): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    log_info "Rollback cancelado"
    exit 0
fi

# Executar rollback
log_info "Executando rollback..."
if doctl apps create-deployment $APP_ID --deployment-id $DEPLOYMENT_ID; then
    log_success "Rollback iniciado com sucesso!"
    log_info "Monitore o progresso no painel da DigitalOcean"
else
    log_error "Erro ao executar rollback"
    exit 1
fi

echo ""
log_info "🔗 Acesse o painel: https://cloud.digitalocean.com/apps/$APP_ID"
