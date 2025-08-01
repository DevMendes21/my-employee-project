#!/bin/bash

# Script para configurar o ambiente de desenvolvimento no Mac
echo "🚀 Configurando o ambiente para o projeto Minha Empresa no Mac..."

# Verificar se o Homebrew está instalado
if ! command -v brew &> /dev/null; then
    echo "📦 Instalando Homebrew..."
    /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
fi

# Instalar MySQL se não estiver instalado
if ! command -v mysql &> /dev/null; then
    echo "🗄️ Instalando MySQL..."
    brew install mysql
    
    echo "▶️ Iniciando o serviço MySQL..."
    brew services start mysql
    
    echo "🔐 Configurando MySQL..."
    echo "Por favor, execute o comando abaixo para configurar a segurança do MySQL:"
    echo "mysql_secure_installation"
else
    echo "✅ MySQL já está instalado"
    # Garantir que o MySQL esteja rodando
    brew services start mysql
fi

# Verificar se o .NET está instalado
if ! command -v dotnet &> /dev/null; then
    echo "⚡ Instalando .NET 8..."
    brew install --cask dotnet
else
    echo "✅ .NET já está instalado"
fi

# Criar o banco de dados
echo "🗃️ Configurando o banco de dados..."
mysql -u root -p << EOF
source EmpresaDB.sql
EOF

# Copiar o arquivo de configuração
if [ ! -f "App.config" ]; then
    echo "📋 Criando arquivo de configuração..."
    cp App.config.template App.config
    echo "⚠️ IMPORTANTE: Edite o arquivo App.config com suas credenciais do MySQL"
fi

# Configurar appsettings.json
if [ ! -f "appsettings.json" ]; then
    echo "⚠️ IMPORTANTE: Configure o arquivo appsettings.json com suas credenciais do MySQL"
    echo "Exemplo:"
    echo '{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;user=root;database=EmpresaDB;password=SUA_SENHA;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}'
fi

echo ""
echo "✅ Configuração concluída!"
echo ""
echo "📝 Próximos passos:"
echo "1. Configure suas credenciais do MySQL no arquivo appsettings.json"
echo "2. Execute: dotnet run --project MinhaEmpresa.Web.csproj"
echo "3. Acesse: http://localhost:5000"
echo ""
echo "🔧 Para a versão Windows Forms (funciona no Mac com limitações):"
echo "1. Configure suas credenciais do MySQL no arquivo App.config"
echo "2. Execute: dotnet run --project MinhaEmpresa.csproj"
echo ""
