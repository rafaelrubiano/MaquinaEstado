-- Eliminar datos existentes (en el orden correcto para evitar errores de integridad referencial)
DELETE FROM HistorialEstados;
DELETE FROM Solicitudes;
DELETE FROM Transiciones;
DELETE FROM Estados;

-- Insertar datos de prueba en la tabla Estados
INSERT INTO Estados (Nombre, CreadoPor)
VALUES 
('Creado', 'admin'),
('En Revisión', 'admin'),
('Aprobado', 'admin'),
('Rechazado', 'admin'),
('Finalizado', 'admin'),
('Pendiente de Documentación', 'admin'),
('Documentación Completa', 'admin'),
('En Proceso', 'admin'),
('Cancelado', 'admin'),
('Archivado', 'admin');

-- Insertar datos de prueba en la tabla Transiciones
INSERT INTO Transiciones (EstadoOrigenId, EstadoDestinoId, CreadoPor)
VALUES 
-- Flujo 1: Creado -> En Revisión -> Aprobado -> Finalizado
(1, 2, 'admin'), -- Creado -> En Revisión
(2, 3, 'admin'), -- En Revisión -> Aprobado
(3, 5, 'admin'), -- Aprobado -> Finalizado

-- Flujo 2: Creado -> En Revisión -> Rechazado -> Finalizado
(1, 2, 'admin'), -- Creado -> En Revisión
(2, 4, 'admin'), -- En Revisión -> Rechazado
(4, 5, 'admin'), -- Rechazado -> Finalizado

-- Flujo 3: Creado -> Pendiente de Documentación -> Documentación Completa -> En Proceso -> Finalizado
(1, 6, 'admin'), -- Creado -> Pendiente de Documentación
(6, 7, 'admin'), -- Pendiente de Documentación -> Documentación Completa
(7, 8, 'admin'), -- Documentación Completa -> En Proceso
(8, 5, 'admin'), -- En Proceso -> Finalizado

-- Flujo 4: Creado -> En Revisión -> Cancelado -> Archivado
(1, 2, 'admin'), -- Creado -> En Revisión
(2, 9, 'admin'), -- En Revisión -> Cancelado
(9, 10, 'admin'), -- Cancelado -> Archivado

-- Flujo 5: Creado -> En Revisión -> Aprobado -> Archivado
(1, 2, 'admin'), -- Creado -> En Revisión
(2, 3, 'admin'), -- En Revisión -> Aprobado
(3, 10, 'admin'); -- Aprobado -> Archivado

-- Insertar datos de prueba en la tabla Solicitudes
INSERT INTO Solicitudes (Descripcion, EstadoId, CreadoPor)
VALUES 
('Solicitud 1 - Flujo 1', 1, 'admin'), -- Estado: Creado
('Solicitud 2 - Flujo 2', 1, 'admin'), -- Estado: Creado
('Solicitud 3 - Flujo 3', 1, 'admin'), -- Estado: Creado
('Solicitud 4 - Flujo 4', 1, 'admin'), -- Estado: Creado
('Solicitud 5 - Flujo 5', 1, 'admin'); -- Estado: Creado

-- Insertar datos de prueba en la tabla HistorialEstados
-- Función para generar un historial de 20 cambios de estado por solicitud
DECLARE @SolicitudId INT, @EstadoAnteriorId INT, @EstadoNuevoId INT, @Usuario NVARCHAR(100), @FechaCambio DATETIME;

-- Solicitud 1: Flujo 1 (Creado -> En Revisión -> Aprobado -> Finalizado)
SET @SolicitudId = 1;
SET @EstadoAnteriorId = 1; -- Creado
SET @EstadoNuevoId = 2; -- En Revisión
SET @Usuario = 'user1';
SET @FechaCambio = DATEADD(DAY, -20, GETUTCDATE()); -- Comienza hace 20 días

INSERT INTO HistorialEstados (SolicitudId, EstadoAnteriorId, EstadoNuevoId, Usuario, FechaCambio, CreadoPor)
VALUES (@SolicitudId, @EstadoAnteriorId, @EstadoNuevoId, @Usuario, @FechaCambio, 'admin');

SET @EstadoAnteriorId = @EstadoNuevoId;
SET @EstadoNuevoId = 3; -- Aprobado
SET @FechaCambio = DATEADD(DAY, 1, @FechaCambio);

INSERT INTO HistorialEstados (SolicitudId, EstadoAnteriorId, EstadoNuevoId, Usuario, FechaCambio, CreadoPor)
VALUES (@SolicitudId, @EstadoAnteriorId, @EstadoNuevoId, @Usuario, @FechaCambio, 'admin');

SET @EstadoAnteriorId = @EstadoNuevoId;
SET @EstadoNuevoId = 5; -- Finalizado
SET @FechaCambio = DATEADD(DAY, 1, @FechaCambio);

INSERT INTO HistorialEstados (SolicitudId, EstadoAnteriorId, EstadoNuevoId, Usuario, FechaCambio, CreadoPor)
VALUES (@SolicitudId, @EstadoAnteriorId, @EstadoNuevoId, @Usuario, @FechaCambio, 'admin');

-- Repetir el proceso para las otras 4 solicitudes con sus respectivos flujos
-- (El código es similar, solo cambian los estados y las fechas)

-- Solicitud 2: Flujo 2 (Creado -> En Revisión -> Rechazado -> Finalizado)
-- Solicitud 3: Flujo 3 (Creado -> Pendiente de Documentación -> Documentación Completa -> En Proceso -> Finalizado)
-- Solicitud 4: Flujo 4 (Creado -> En Revisión -> Cancelado -> Archivado)
-- Solicitud 5: Flujo 5 (Creado -> En Revisión -> Aprobado -> Archivado)

-- Verificar los datos insertados
SELECT * FROM Estados;
SELECT * FROM Transiciones;
SELECT * FROM Solicitudes;
SELECT * FROM HistorialEstados;