-- Estrutura do banco usada por Equilibrium/Data/AppDbContext.cs
-- Execute no MySQL Workbench ou com: mysql -u root -p < .sql/01_criar_banco.sql

CREATE DATABASE IF NOT EXISTS sistema_saude_mental
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE sistema_saude_mental;

CREATE TABLE IF NOT EXISTS usuarios (
    Id INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(100) NOT NULL,
    CPF VARCHAR(14) NULL,
    Email VARCHAR(100) NOT NULL,
    Senha VARCHAR(255) NOT NULL,
    Tipo VARCHAR(20) NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY IX_usuarios_Email (Email),
    UNIQUE KEY IX_usuarios_CPF (CPF)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Herança TPT: cada tipo de usuário compartilha o Id da tabela usuarios.
CREATE TABLE IF NOT EXISTS pacientes (
    Id INT NOT NULL,
    Idade INT NOT NULL,
    Endereco VARCHAR(200) NOT NULL,
    Telefone VARCHAR(20) NOT NULL,
    PRIMARY KEY (Id),
    CONSTRAINT FK_pacientes_usuarios FOREIGN KEY (Id)
        REFERENCES usuarios (Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS medicos (
    Id INT NOT NULL,
    CRM VARCHAR(30) NOT NULL,
    Especialidade VARCHAR(100) NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY IX_medicos_CRM (CRM),
    CONSTRAINT FK_medicos_usuarios FOREIGN KEY (Id)
        REFERENCES usuarios (Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS administradores (
    Id INT NOT NULL,
    NivelAcesso VARCHAR(50) NOT NULL,
    PRIMARY KEY (Id),
    CONSTRAINT FK_administradores_usuarios FOREIGN KEY (Id)
        REFERENCES usuarios (Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS prontuarios (
    Id INT NOT NULL AUTO_INCREMENT,
    PacienteId INT NOT NULL,
    Observacoes TEXT NOT NULL,
    DataAtendimento DATETIME(6) NOT NULL,
    PRIMARY KEY (Id),
    KEY IX_prontuarios_PacienteId (PacienteId),
    CONSTRAINT FK_prontuarios_pacientes FOREIGN KEY (PacienteId)
        REFERENCES pacientes (Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS consultas (
    Id INT NOT NULL AUTO_INCREMENT,
    PacienteId INT NOT NULL,
    MedicoId INT NOT NULL,
    DataHora DATETIME(6) NOT NULL,
    Status VARCHAR(20) NOT NULL,
    Observacoes VARCHAR(500) NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY IX_consultas_MedicoId_DataHora (MedicoId, DataHora),
    KEY IX_consultas_PacienteId (PacienteId),
    CONSTRAINT FK_consultas_pacientes FOREIGN KEY (PacienteId)
        REFERENCES pacientes (Id) ON DELETE RESTRICT,
    CONSTRAINT FK_consultas_medicos FOREIGN KEY (MedicoId)
        REFERENCES medicos (Id) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
