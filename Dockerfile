# Dockerfile para o Sistema de Funcionários
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos do projeto
COPY ["MinhaEmpresa.Web.csproj", "./"]
COPY ["Models/", "./Models/"]
COPY ["DAO/", "./DAO/"]
COPY ["Conexao/", "./Conexao/"]
COPY ["Utils/", "./Utils/"]
COPY ["Controllers/", "./Controllers/"]
COPY ["Views/", "./Views/"]
COPY ["appsettings.json", "./"]

# Restaurar dependências
RUN dotnet restore "MinhaEmpresa.Web.csproj"

# Copiar todo o código
COPY . .

# Build da aplicação
RUN dotnet build "MinhaEmpresa.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MinhaEmpresa.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Instalar cliente MySQL para conexão
RUN apt-get update && apt-get install -y default-mysql-client

ENTRYPOINT ["dotnet", "MinhaEmpresa.Web.dll"]
