# Sistema de Gestão de Funcionários

Sistema desktop desenvolvido em C# Windows Forms para gerenciamento de funcionários, utilizando banco de dados MySQL.

## Requisitos

### Windows
- Visual Studio 2019 ou superior
- MySQL Server 8.0 ou superior
- .NET 6.0 SDK ou superior

### macOS
1. Instale o Homebrew (se ainda não tiver):
   ```bash
   /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
   ```

2. Instale o .NET SDK:
   ```bash
   brew install dotnet-sdk
   ```

3. Instale o MySQL:
   ```bash
   brew install mysql
   ```

## Configuração e Execução

### 1. Banco de Dados
- Execute o script SQL para criar o banco:
  ```bash
  mysql -u root -p < EmpresaDB.sql
  ```

### 2. Configuração da Conexão
1. Copie o arquivo `App.config.template` e nomeie para `App.config`
2. No arquivo `App.config`, configure suas credenciais do banco:
   ```xml
   connectionString="server=localhost;user=SEU_USUARIO;database=EmpresaDB;password=SUA_SENHA;"
   ```
3. O arquivo `App.config` não será versionado no Git por questões de segurança

### 3. Executando o Projeto

#### No Visual Studio:
1. Abra o arquivo `MinhaEmpresa.sln`
2. No Solution Explorer, clique com botão direito na solução
3. Selecione "Restore NuGet Packages"
4. Pressione F5 ou clique no botão Play para executar

#### Via Terminal (macOS/Linux):
```bash
# Instalar dependências
dotnet restore

# Compilar o projeto
dotnet build

# Executar
dotnet run --project MinhaEmpresa.csproj
```

**Nota**: No macOS, você precisará usar o Visual Studio Code ou Rider para desenvolvimento, já que o Visual Studio só está disponível para Windows.

### 4. Arquivos Principais
- `MinhaEmpresa.sln`: Arquivo da solução
- `Program.cs`: Ponto de entrada da aplicação
- `Menu.cs`: Tela principal do sistema

## Funcionalidades

- Cadastro de funcionários
- Listagem em tabela
- Edição de dados
- Remoção de registros

## Estrutura do Projeto

```
MinhaEmpresa/
├── Conexao/     # Gerenciamento de conexão com banco
├── Models/      # Classes de modelo
├── DAO/         # Acesso a dados
├── Views/       # Formulários da interface
└── Program.cs   # Ponto de entrada
```

## Arquivos que eu preferi fazer todo em Ingles

```
MenuPrincipal.cs
Menu.Designer.cs
```