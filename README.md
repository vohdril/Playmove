 - Antes de executar a API, é necessário:
1) Criar a base de dados conforme o script SQL abaixo:

	CREATE DATABASE FornecedoresDB
	GO
	
	USE FORNECEDORESDB
	GO
	
	CREATE TABLE Fornecedor
		(
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

3) Executar o comando "update-database" no Package Manager Console, apontando para o projeto "Playmove.DAO" (isso ira incluir as tabelas Identity usadas pra autenticação do usuário)
