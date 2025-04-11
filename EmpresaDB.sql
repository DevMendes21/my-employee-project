CREATE DATABASE EmpresaDB;
USE EmpresaDB;

CREATE TABLE funcionarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100),
    cargo VARCHAR(50),
    salario DECIMAL(10,2),
    departamento VARCHAR(50),
    data_contratacao DATE
);
