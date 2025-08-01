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
    
    # Verificar se Docker está rodando
    if ! docker info > /dev/null 2>&1; then
        log_warning "Docker não está rodando!"
        log_info "Para iniciar o Docker:"
        log_info "1. Abra o Docker Desktop"
        log_info "2. Aguarde o Docker inicializar"
        log_info "3. Execute este script novamente"
        echo ""
        read -p "Continuar sem testar Docker? (Y/n): " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Nn]$ ]]; then
            log_info "Deploy cancelado. Inicie o Docker e tente novamente."
            exit 1
        fi
        log_warning "Continuando sem teste Docker local..."
    else
        if docker build -t minha-empresa-test . > /dev/null 2>&1; then
            log_success "Docker build OK"
            docker rmi minha-empresa-test > /dev/null 2>&1
        else
            log_error "Erro no Docker build"
            log_info "Executando build com output detalhado..."
            docker build -t minha-empresa-test .
            exit 1
        fi
    fi
fi

# 5. Verificar arquivos de configuração sensíveis
log_warning "🔒 VERIFICANDO SEGURANÇA..."

# Verificar se dados sensíveis não estão commitados (apenas arquivos rastreados pelo Git)
log_info "Verificando arquivos rastreados pelo Git..."
if git ls-files | xargs grep -l "corelink-db\|AVNS_" 2>/dev/null | grep -v -E "\.(sh|md)$"; then
    log_error "DADOS SENSÍVEIS ENCONTRADOS EM ARQUIVOS RASTREADOS!"
    log_error "Os arquivos acima contêm dados sensíveis e estão no Git"
    log_info "Removendo do Git e adicionando ao .gitignore..."
    
    # Remover arquivos sensíveis do Git
    git ls-files | xargs grep -l "corelink-db\|AVNS_" 2>/dev/null | grep -v -E "\.(sh|md)$" | while read -r file; do
        log_warning "Removendo $file do Git"
        git rm --cached "$file" 2>/dev/null || true
    done
    
    log_info "Arquivos removidos do Git. Continue o deploy."
else
    log_success "Nenhum dado sensível encontrado em arquivos rastreados"
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

# Verificar se app.yaml tem dados sensíveis (só se estiver rastreado pelo Git)
if [ -f ".do/app.yaml" ]; then
    if git ls-files --error-unmatch .do/app.yaml >/dev/null 2>&1; then
        # Arquivo está rastreado pelo Git, verificar conteúdo
        if grep -q "corelink-db\|AVNS_" .do/app.yaml 2>/dev/null; then
            log_error "Dados sensíveis encontrados em .do/app.yaml rastreado pelo Git!"
            log_info "Removendo do Git e usando template..."
            git rm --cached .do/app.yaml
        fi
    else
        # Arquivo não está rastreado pelo Git (correto)
        log_success ".do/app.yaml não está rastreado pelo Git (correto)"
    fi
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
echo "6. ⚙️  Configure as variáveis de ambiente na DigitalOcean:"
echo "   - DB_HOST, DB_PORT, DB_USER, DB_NAME, DB_PASSWORD, DB_SSL_MODE"
echo ""
echo "7. 🚀 Clique em 'Create Resources'"
echo ""
echo "======================================================"
log_info "🔗 Sua aplicação estará disponível em:"
log_info "https://minha-empresa-system-xxxxx.ondigitalocean.app"
echo "======================================================"
echo ""
log_info "🐳 DICA: Se houve problema com Docker:"
log_info "Execute: ./test-docker.sh para diagnosticar"
echo ""
log_warning "⏱️  O deploy pode levar de 5-10 minutos"
log_info "📊 Monitore o progresso no painel da DigitalOcean"
echo ""
log_success "🎉 Deploy iniciado com sucesso!"
