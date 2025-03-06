USE master; -- Cambiamos al contexto de la base de datos maestra
GO

-- Verificar si la base de datos existe y eliminarla si es necesario
IF DB_ID('MaquinaEstadosDB') IS NOT NULL
BEGIN
    PRINT 'La base de datos MaquinaEstadosDB existe. Eliminándola...';
    ALTER DATABASE MaquinaEstadosDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE; -- Forzar cierre de conexiones
    DROP DATABASE MaquinaEstadosDB;
    PRINT 'Base de datos MaquinaEstadosDB eliminada.';
END
ELSE
BEGIN
    PRINT 'La base de datos MaquinaEstadosDB no existe.';
END

-- Crear la base de datos
PRINT 'Creando la base de datos MaquinaEstadosDB...';
CREATE DATABASE MaquinaEstadosDB;
PRINT 'Base de datos MaquinaEstadosDB creada.';
GO

-- Usar la base de datos recién creada
USE MaquinaEstadosDB;
GO

-- Crear la tabla Estados
PRINT 'Creando la tabla Estados...';
CREATE TABLE Estados (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(255) NOT NULL,
    Creado DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CreadoPor NVARCHAR(100) NOT NULL,
    Modificado DATETIME NULL,
    ModificadoPor NVARCHAR(100) NULL,
    Activo BIT NOT NULL DEFAULT 1
);
PRINT 'Tabla Estados creada.';
GO

-- Crear la tabla Transiciones
PRINT 'Creando la tabla Transiciones...';
CREATE TABLE Transiciones (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EstadoOrigenId INT NOT NULL,
    EstadoDestinoId INT NOT NULL,
    Creado DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CreadoPor NVARCHAR(100) NOT NULL,
    Modificado DATETIME NULL,
    ModificadoPor NVARCHAR(100) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (EstadoOrigenId) REFERENCES Estados(Id),
    FOREIGN KEY (EstadoDestinoId) REFERENCES Estados(Id)
);
PRINT 'Tabla Transiciones creada.';
GO

-- Crear la tabla Solicitudes
PRINT 'Creando la tabla Solicitudes...';
CREATE TABLE Solicitudes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Descripcion NVARCHAR(500) NOT NULL,
    EstadoId INT NOT NULL,
    Creado DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CreadoPor NVARCHAR(100) NOT NULL,
    Modificado DATETIME NULL,
    ModificadoPor NVARCHAR(100) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (EstadoId) REFERENCES Estados(Id)
);
PRINT 'Tabla Solicitudes creada.';
GO

-- Crear la tabla HistorialEstados
PRINT 'Creando la tabla HistorialEstados...';
CREATE TABLE HistorialEstados (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SolicitudId INT NOT NULL,
    EstadoAnteriorId INT NOT NULL,
    EstadoNuevoId INT NOT NULL,
    Usuario NVARCHAR(100) NOT NULL,
    FechaCambio DATETIME NOT NULL DEFAULT GETUTCDATE(),
    Creado DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CreadoPor NVARCHAR(100) NOT NULL,
    Modificado DATETIME NULL,
    ModificadoPor NVARCHAR(100) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (SolicitudId) REFERENCES Solicitudes(Id),
    FOREIGN KEY (EstadoAnteriorId) REFERENCES Estados(Id),
    FOREIGN KEY (EstadoNuevoId) REFERENCES Estados(Id)
);
PRINT 'Tabla HistorialEstados creada.';
GO

-- Crear índices para optimizar consultas
PRINT 'Creando índices...';
CREATE INDEX IX_Solicitudes_EstadoId ON Solicitudes(EstadoId) WHERE Activo = 1;
CREATE INDEX IX_Transiciones_Origen ON Transiciones(EstadoOrigenId) WHERE Activo = 1;
CREATE INDEX IX_Transiciones_Destino ON Transiciones(EstadoDestinoId) WHERE Activo = 1;
CREATE INDEX IX_HistorialEstados_SolicitudId ON HistorialEstados(SolicitudId) WHERE Activo = 1;
PRINT 'Índices creados.';
GO

PRINT 'Script ejecutado con éxito.';