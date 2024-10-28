 - Antes de executar a API, é apenas necessário criar a base de dados conforme o script SQL abaixo:

CREATE DATABASE FornecedoresDB
GO

USE FORNECEDORESDB
GO

CREATE TABLE Fornecedor(
		Id INT  IDENTITY(1,1) PRIMARY KEY,
		NOME VARCHAR  (255),
		EMAIL VARCHAR  (255),
		CNPJ VARCHAR(18) UNIQUE NOT NULL,               
		TELEFONE VARCHAR(15) NOT NULL,                   
		ENDERECO VARCHAR(255) NOT NULL,                 
		CIDADE VARCHAR(100) NOT NULL,                  
		ESTADO VARCHAR(2) NOT NULL,                    
		CEP VARCHAR(9) NOT NULL,                       
		DATA_CADASTRO DATETIME NOT NULL DEFAULT GETDATE(), 
		STATUS BIT NOT NULL DEFAULT 0           
)
