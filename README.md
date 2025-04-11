# Sistema de Gestão de Funcionários

Sistema desktop desenvolvido em C# Windows Forms para gerenciamento de funcionários, utilizando banco de dados MySQL.

## Requisitos

- Visual Studio 2019 ou superior
- MySQL Server 8.0 ou superior
- .NET Framework 4.7.2 ou superior

## Configuração Rápida

1. Clone o repositório
2. Execute o script `EmpresaDB.sql` no MySQL
3. Ajuste a string de conexão em `Conexao/Conexao.cs`
4. Abra a solução no Visual Studio e execute

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
