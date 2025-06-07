-- =================================================================================
-- CRIA��O DO BANCO DE DADOS
-- Cria o banco de dados 'bdEmpresa' somente se ele ainda n�o existir.
-- =================================================================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'bdEmpresa')
BEGIN
    CREATE DATABASE bdEmpresa;
END
GO

-- Muda o contexto da execu��o para o banco de dados rec�m-criado.
USE bdEmpresa;
GO

-- =================================================================================
-- TABELA DE DEPARTAMENTOS
-- Tabela independente, n�o possui chaves estrangeiras.
-- =================================================================================
CREATE TABLE Departamentos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(200) NOT NULL,
    Sigla NVARCHAR(10) NOT NULL,
    Email NVARCHAR(100) NULL,
    Telefone NVARCHAR(20) NULL
);
GO

-- =================================================================================
-- TABELA DE CIDADES
-- Tabela independente, n�o possui chaves estrangeiras.
-- =================================================================================
CREATE TABLE Cidades (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(200) NOT NULL,
    Estado NVARCHAR(100) NOT NULL,
    UF NCHAR(2) NOT NULL,
    CEP NVARCHAR(10) NULL,
    Populacao INT NULL,
    CONSTRAINT CHK_Populacao_Nao_Negativa CHECK (Populacao >= 0) -- Garante que a popula��o n�o seja negativa.
);
GO

-- =================================================================================
-- TABELA DE CARGOS
-- Depende da tabela de Departamentos.
-- =================================================================================
CREATE TABLE Cargos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DepartamentoId INT NOT NULL,
    Nome NVARCHAR(200) NOT NULL,
    Descricao NVARCHAR(500) NULL,
    SalarioBase DECIMAL(18, 2) NOT NULL,
    
    -- Defini��o da chave estrangeira para Departamentos
    CONSTRAINT FK_Cargos_Departamentos FOREIGN KEY (DepartamentoId) REFERENCES Departamentos(Id)
        ON DELETE NO ACTION -- Impede a exclus�o de um departamento que tenha cargos associados.
        ON UPDATE CASCADE,  -- Se o Id de um departamento for alterado, atualiza aqui tamb�m.

    -- Garante que o sal�rio base seja um valor positivo.
    CONSTRAINT CHK_SalarioBase_Positivo CHECK (SalarioBase > 0)
);
GO

-- =================================================================================
-- TABELA DE FUNCION�RIOS
-- Tabela central, depende de Departamentos, Cargos e Cidades.
-- =================================================================================
CREATE TABLE Funcionarios (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DepartamentoId INT NOT NULL,
    CargoId INT NOT NULL,
    CidadeId INT NULL, -- Permite nulos, conforme a entidade (int?).

    -- Dados Pessoais
    Nome NVARCHAR(200) NOT NULL,
    NomeMae NVARCHAR(200) NULL,
    NomePai NVARCHAR(200) NULL,
    DataNascimento DATE NOT NULL, -- Usamos DATE pois a hora n�o � relevante.
    CPF NCHAR(11) NOT NULL,
    RG NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,

    -- Dados Profissionais
    Salario DECIMAL(18, 2) NOT NULL,
    DataAdmissao DATE NOT NULL DEFAULT GETDATE(), -- Valor padr�o � a data atual.
    Ativo BIT NOT NULL DEFAULT 1, -- Valor padr�o � 'true' (ativo).

    -- Adiciona uma restri��o de unicidade para o CPF.
    CONSTRAINT UQ_Funcionarios_CPF UNIQUE (CPF),

    -- Garante que o sal�rio seja um valor positivo.
    CONSTRAINT CHK_Salario_Positivo CHECK (Salario > 0),

    -- Defini��o das chaves estrangeiras
    CONSTRAINT FK_Funcionarios_Departamentos FOREIGN KEY (DepartamentoId) REFERENCES Departamentos(Id)
        ON DELETE NO ACTION ON UPDATE CASCADE,
    
    CONSTRAINT FK_Funcionarios_Cargos FOREIGN KEY (CargoId) REFERENCES Cargos(Id)
        ON DELETE NO ACTION ON UPDATE NO ACTION, -- Evita complexidade de cascata se um cargo for movido de departamento.
    
    CONSTRAINT FK_Funcionarios_Cidades FOREIGN KEY (CidadeId) REFERENCES Cidades(Id)
        ON DELETE SET NULL -- Se uma cidade for removida, o campo no funcion�rio se torna nulo.
        ON UPDATE CASCADE
);
GO

PRINT 'Banco de dados "bdEmpresa" e tabelas criados com sucesso!';
GO
/////// 

public class Funcionario
 {
     // ========================================
     // IDENTIDADE E RELACIONAMENTOS
     // ========================================
     public int Id { get; set; }
     public int DepartamentoId { get; set; }
     public int CargoId { get; set; }
     public int? CidadeId { get; set; }  // Nullable - opcional

     // ========================================
     // DADOS PESSOAIS
     // ========================================
     [Required(ErrorMessage = "Nome � obrigat�rio")]
     [StringLength(200, ErrorMessage = "Nome deve ter no m�ximo 200 caracteres")]
     public string Nome { get; set; } = string.Empty;

     [StringLength(200)]
     public string NomeMae { get; set; } = string.Empty;

     [StringLength(200)]
     public string NomePai { get; set; } = string.Empty;

     [Required(ErrorMessage = "Data de nascimento � obrigat�ria")]
     public DateTime DataNascimento { get; set; }

     [Required(ErrorMessage = "CPF � obrigat�rio")]
     [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter exatamente 11 d�gitos")]
     public string CPF { get; set; } = string.Empty;

     [StringLength(20)]
     public string RG { get; set; } = string.Empty;

     [EmailAddress(ErrorMessage = "Email inv�lido")]
     [StringLength(100)]
     public string Email { get; set; } = string.Empty;

     // ========================================
     // DADOS PROFISSIONAIS
     // ========================================
     [Required(ErrorMessage = "Sal�rio � obrigat�rio")]
     [Range(0.01, double.MaxValue, ErrorMessage = "Sal�rio deve ser maior que zero")]
     public decimal Salario { get; set; }

     [Required(ErrorMessage = "Data de admiss�o � obrigat�ria")]
     public DateTime DataAdmissao { get; set; } = DateTime.Today;

     public bool Ativo { get; set; } = true;

     // ========================================
     // CAMPOS PREENCHIDOS VIA JOIN (ADO.NET)
     // Estes campos s�o populados pelas queries SQL
     // ========================================
     public string NomeDepartamento { get; set; } = string.Empty;
     public string NomeCargo { get; set; } = string.Empty;
     public string NomeCidade { get; set; } = string.Empty;

     // ========================================
     // OBJETOS DE NAVEGA��O (Para uso futuro)
     // ========================================
     public Departamento? Departamento { get; set; }
     public Cargo? Cargo { get; set; }
     public Cidade? Cidade { get; set; }
 }



-- =================================================================================
-- CRIA��O DO BANCO DE DADOS
-- Cria o banco de dados 'bdEmpresa' somente se ele ainda n�o existir.
-- =================================================================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'bdEmpresa')
BEGIN
    CREATE DATABASE bdEmpresa;
END
GO

-- Muda o contexto da execu��o para o banco de dados rec�m-criado.
USE bdEmpresa;
GO

-- =================================================================================
-- TABELA DE DEPARTAMENTOS
-- Tabela independente, n�o possui chaves estrangeiras.
-- =================================================================================
CREATE TABLE Departamentos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(200) NOT NULL,
    Sigla NVARCHAR(10) NOT NULL,
    Email NVARCHAR(100) NULL,
    Telefone NVARCHAR(20) NULL
);
GO

-- =================================================================================
-- TABELA DE CIDADES
-- Tabela independente, n�o possui chaves estrangeiras.
-- =================================================================================
CREATE TABLE Cidades (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(200) NOT NULL,
    Estado NVARCHAR(100) NOT NULL,
    UF NCHAR(2) NOT NULL,
    CEP NVARCHAR(10) NULL,
    Populacao INT NULL,
    CONSTRAINT CHK_Populacao_Nao_Negativa CHECK (Populacao >= 0) -- Garante que a popula��o n�o seja negativa.
);
GO

-- =================================================================================
-- TABELA DE CARGOS
-- Depende da tabela de Departamentos.
-- =================================================================================
CREATE TABLE Cargos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DepartamentoId INT NOT NULL,
    Nome NVARCHAR(200) NOT NULL,
    Descricao NVARCHAR(500) NULL,
    SalarioBase DECIMAL(18, 2) NOT NULL,
    
    -- Defini��o da chave estrangeira para Departamentos
    CONSTRAINT FK_Cargos_Departamentos FOREIGN KEY (DepartamentoId) REFERENCES Departamentos(Id)
        ON DELETE NO ACTION -- Impede a exclus�o de um departamento que tenha cargos associados.
        ON UPDATE CASCADE,  -- Se o Id de um departamento for alterado, atualiza aqui tamb�m.

    -- Garante que o sal�rio base seja um valor positivo.
    CONSTRAINT CHK_SalarioBase_Positivo CHECK (SalarioBase > 0)
);
GO

-- =================================================================================
-- TABELA DE FUNCION�RIOS
-- Tabela central, depende de Departamentos, Cargos e Cidades.
-- =================================================================================
CREATE TABLE Funcionarios (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DepartamentoId INT NOT NULL,
    CargoId INT NOT NULL,
    CidadeId INT NULL, -- Permite nulos, conforme a entidade (int?).

    -- Dados Pessoais
    Nome NVARCHAR(200) NOT NULL,
    NomeMae NVARCHAR(200) NULL,
    NomePai NVARCHAR(200) NULL,
    DataNascimento DATE NOT NULL, -- Usamos DATE pois a hora n�o � relevante.
    CPF NCHAR(11) NOT NULL,
    RG NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,

    -- Dados Profissionais
    Salario DECIMAL(18, 2) NOT NULL,
    DataAdmissao DATE NOT NULL DEFAULT GETDATE(), -- Valor padr�o � a data atual.
    Ativo BIT NOT NULL DEFAULT 1, -- Valor padr�o � 'true' (ativo).

    -- Adiciona uma restri��o de unicidade para o CPF.
    CONSTRAINT UQ_Funcionarios_CPF UNIQUE (CPF),

    -- Garante que o sal�rio seja um valor positivo.
    CONSTRAINT CHK_Salario_Positivo CHECK (Salario > 0),

    -- Defini��o das chaves estrangeiras
    CONSTRAINT FK_Funcionarios_Departamentos FOREIGN KEY (DepartamentoId) REFERENCES Departamentos(Id)
        ON DELETE NO ACTION ON UPDATE CASCADE,
    
    CONSTRAINT FK_Funcionarios_Cargos FOREIGN KEY (CargoId) REFERENCES Cargos(Id)
        ON DELETE NO ACTION ON UPDATE NO ACTION, -- Evita complexidade de cascata se um cargo for movido de departamento.
    
    CONSTRAINT FK_Funcionarios_Cidades FOREIGN KEY (CidadeId) REFERENCES Cidades(Id)
        ON DELETE SET NULL -- Se uma cidade for removida, o campo no funcion�rio se torna nulo.
        ON UPDATE CASCADE
);
GO

PRINT 'Banco de dados "bdEmpresa" e tabelas criados com sucesso!';
GO