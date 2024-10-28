 - Antes de executar a API, é apenas necessário criar a base de dados conforme o script SQL abaixo:

CREATE DATABASE FornecedoresDB
GO

USE FORNECEDORESDB
GO

CREATE TABLE Fornecedor(
		Id INT  IDENTITY(1,1) PRIMARY KEY,
		NOME VARCHAR  (255),
		EMAIL VARCHAR  (255),
		CNPJ VARCHAR(18) UNIQUE NOT NULL,               -- CNPJ do fornecedor (exclusivo no Brasil)
		TELEFONE VARCHAR(15) NOT NULL,                      -- Telefone de contato
		ENDERECO VARCHAR(255) NOT NULL,                 -- Endereço completo
		CIDADE VARCHAR(100) NOT NULL,                   -- Cidade do fornecedor
		ESTADO VARCHAR(2) NOT NULL,                     -- Sigla do estado (ex: SP, RJ)
		CEP VARCHAR(9) NOT NULL,                        -- Código postal (CEP)
		DATA_CADASTRO DATETIME NOT NULL DEFAULT GETDATE(), -- Data de cadastro do fornecedor
		STATUS BIT NOT NULL DEFAULT 0            -- Status ativo/inativo
)
