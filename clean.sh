#!/bin/bash

# 🧹 Script de Limpeza - Remove dados sensíveis temporários
# Execute antes de commits para garantir segurança

echo "🧹 Limpando dados sensíveis temporários..."

# Remover diretórios de build
rm -rf bin/ obj/ 2>/dev/null || true
echo "✅ Diretórios bin/ e obj/ removidos"

# Remover arquivos de configuração gerados
rm -f *.dll.config 2>/dev/null || true
echo "✅ Arquivos .dll.config removidos"

# Verificar se arquivos sensíveis existem mas não estão no .gitignore
echo ""
echo "🔍 Verificando arquivos sensíveis..."

SENSITIVE_FILES=(".env" "App.config" ".do/app.yaml")
for file in "${SENSITIVE_FILES[@]}"; do
    if [ -f "$file" ]; then
        if git check-ignore "$file" > /dev/null 2>&1; then
            echo "✅ $file - Protegido pelo .gitignore"
        else
            echo "⚠️  $file - ATENÇÃO: Não está no .gitignore!"
        fi
    fi
done

echo ""
echo "✅ Limpeza concluída!"
