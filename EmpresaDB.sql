DROP DATABASE IF EXISTS EmpresaDB;
CREATE DATABASE EmpresaDB;
USE EmpresaDB;

-- Tabela de Departamentos
CREATE TABLE departamentos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE,
    descricao TEXT,
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de Cargos (com chave única composta)
CREATE TABLE cargos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL,
    nivel VARCHAR(20),
    salario_base DECIMAL(10,2),
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uk_cargo_nivel (nome, nivel)
);

-- Tabela de Funcionários com relacionamentos
CREATE TABLE funcionarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE,
    telefone VARCHAR(20),
    cargo_id INT NOT NULL,
    departamento_id INT NOT NULL,
    salario DECIMAL(10,2) NOT NULL,
    data_contratacao DATE NOT NULL,
    data_nascimento DATE,
    status ENUM('Ativo', 'Inativo', 'Ferias', 'Licenca') DEFAULT 'Ativo',
    observacoes TEXT,
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (cargo_id) REFERENCES cargos(id),
    FOREIGN KEY (departamento_id) REFERENCES departamentos(id)
);

-- Inserir dados iniciais
INSERT INTO departamentos (nome, descricao) VALUES
('TI', 'Tecnologia da Informação'),
('RH', 'Recursos Humanos'),
('Financeiro', 'Departamento Financeiro'),
('Comercial', 'Vendas e Marketing'),
('Administrativo', 'Administração Geral');

-- Inserir cargos com níveis diferentes
INSERT INTO cargos (nome, nivel, salario_base) VALUES
('Analista', 'Júnior', 3500.00),
('Analista', 'Pleno', 5500.00),
('Analista', 'Sênior', 8500.00),
('Desenvolvedor', 'Júnior', 4000.00),
('Desenvolvedor', 'Pleno', 6500.00),
('Desenvolvedor', 'Sênior', 9500.00),
('Gerente', 'Sênior', 12000.00),
('Coordenador', 'Pleno', 8000.00),
('Assistente', 'Júnior', 2500.00);
