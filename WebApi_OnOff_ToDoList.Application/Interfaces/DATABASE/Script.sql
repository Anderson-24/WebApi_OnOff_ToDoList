use OnOff
/*DDL
CREATE TABLE Tbl_StatusTask (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(150)
);
CREATE TABLE Tbl_User (
    id INT IDENTITY(1,1) PRIMARY KEY,
    fullName NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL UNIQUE,
    passwordHash NVARCHAR(255) NOT NULL,
    createdAt DATETIME2 DEFAULT SYSDATETIME(),
    isActive BIT DEFAULT 1
);
CREATE TABLE Tbl_Task (
    id INT IDENTITY(1,1) PRIMARY KEY,
    title NVARCHAR(100) NOT NULL,
    description NVARCHAR(250),
    idStatus INT NOT NULL,
    idUser INT NOT NULL,
    createdAt DATETIME2 DEFAULT SYSDATETIME(),
    updatedAt DATETIME2 NULL,
    CONSTRAINT FK_Task_Status FOREIGN KEY (idStatus) REFERENCES Tbl_statusTask(id),
    CONSTRAINT FK_Task_User FOREIGN KEY (idUser) REFERENCES Tbl_user(id)
);
*/
/*DML
INSERT INTO Tbl_statusTask (name, description)
VALUES
('Bloqueado', 'Tarea detenida por dependencia o error'),
('Por Hacer', 'Pendiente de iniciar'),
('En Curso', 'Actualmente en desarrollo'),
('QA', 'En revisión de calidad'),
('Listo', 'Finalizada y aprobada');

INSERT INTO TBL_USER (fullName, email, passwordHash, createdAt, isActive)
    VALUES ('admin', 'admin@onoff.com', 'admin123', SYSDATETIME(), 1);

INSERT INTO TBL_TASK (title, description, idUser, idStatus, createdAt, updatedAt)
VALUES 
('Revisar autenticación JWT', 'Validar que el token se genere y se consuma correctamente en Angular.', 1, 1, SYSDATETIME(), NULL),
('Diseñar interfaz de login', 'Agregar logo, estilos con Tailwind y manejo de errores.', 1, 2, SYSDATETIME(), NULL),
('Crear servicio de tareas', 'Implementar métodos CRUD y conexión al backend.', 1, 3, SYSDATETIME(), NULL),
('Configurar TailwindCSS', 'Verificar responsive y colores corporativos.', 1, 1, SYSDATETIME(), NULL),
('Diseñar dashboard principal', 'Mostrar métricas de tareas pendientes y completadas.', 1, 2, SYSDATETIME(), NULL),
('Implementar sidebar dinámico', 'Agregar animaciones y control de colapso.', 1, 3, SYSDATETIME(), NULL),
('Integrar PrimeNG', 'Configurar componentes de tabla y botones.', 1, 1, SYSDATETIME(), NULL),
('Optimizar consultas SQL', 'Revisar índices y crear SP_TASK para filtros.', 1, 2, SYSDATETIME(), NULL),
('Agregar notificaciones', 'Incluir componente de toasts y alertas de validación.', 1, 1, SYSDATETIME(), NULL),
('Crear componente TaskEditCreate', 'Permitir crear y editar tareas desde un diálogo.', 1, 1, SYSDATETIME(), NULL),
('Validar formulario de tareas', 'Asegurar que los campos requeridos estén completos.', 1, 3, SYSDATETIME(), NULL),
('Ajustar tema claro Lara', 'Cambiar configuración del tema para modo claro.', 1, 2, SYSDATETIME(), NULL),
('Configurar proxy en Angular', 'Permitir conexión con backend sin CORS.', 1, 1, SYSDATETIME(), NULL),
('Agregar filtrado a tabla', 'Filtrar tareas por usuario, correo o título.', 1, 3, SYSDATETIME(), NULL),
('Incluir métricas en dashboard', 'Total de tareas, completadas y pendientes.', 1, 2, SYSDATETIME(), NULL),
('Implementar paginación', 'Agregar lazy loading en tabla de tareas.', 1, 1, SYSDATETIME(), NULL),
('Testear endpoints en Swagger', 'Verificar CRUD de tareas y autenticación.', 1, 2, SYSDATETIME(), NULL),
('Crear servicio global de estado', 'Gestionar usuario y token con signals.', 1, 1, SYSDATETIME(), NULL),
('Agregar validaciones visuales', 'Resaltar campos requeridos en rojo y mostrar mensajes.', 1, 3, SYSDATETIME(), NULL),
('Depurar build en Angular', 'Resolver errores de importación y rutas de estilos.', 1, 2, SYSDATETIME(), NULL);
*/

Select* from TBL_STATUSTASK
select * from TBL_task
select * from Tbl_User

select u.fullName, u.email, t.*, s.name
from TBL_task t
Inner join Tbl_User u on t.idUser = u.id
inner join TBL_STATUSTASK s on t.idStatus = s.id

