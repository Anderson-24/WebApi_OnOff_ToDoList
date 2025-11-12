# 🧩 WebApi_OnOff_ToDoList

## 📘 Descripción general

**WebApi_OnOff_ToDoList** es un microservicio REST desarrollado en **.NET 9** que expone un conjunto de endpoints para la gestión de tareas (ToDo List) con autenticación JWT y conexión a base de datos SQL Server.

Este servicio es el backend del proyecto **OnOff ToDoList**, encargado de manejar las operaciones CRUD sobre tareas, usuarios y estados, además de proveer endpoints seguros para autenticación y métricas del sistema.

---

## ⚙️ Arquitectura y tecnologías utilizadas

El proyecto sigue una **arquitectura en capas (Clean Architecture / DDD)** dividida en:
---
WebApi_OnOff_ToDoList/
│
├── WebApi_OnOff_ToDoList.Api/ → Capa de presentación (controladores REST)
│ ├── Controllers/ → Endpoints: Auth, Tasks, Users, StatusTasks
│ └── Program.cs → Configuración general de la API
│
├── WebApi_OnOff_ToDoList.Application/ → Capa de aplicación (servicios y lógica de negocio)
│ ├── Services/ → Casos de uso, validaciones y flujos
│ ├── Task/Queries/ → Consultas SQL y objetos DTO
│ └── Database/ → 📂 Scripts SQL (DDL, DML y SP)
│
├── WebApi_OnOff_ToDoList.Infrastructure/ → Capa de infraestructura
│ ├── Context/ → AppDbContext (EF Core)
│ ├── Entities/ → Modelos de datos (TblUser, TblTask, TblStatusTask)
│ └── Repositories/ → Implementaciones y conexión SQL
│
└── WebApi_OnOff_ToDoList.Domain/ → Entidades base y modelos compartidos
---

---

## 💡 Decisiones técnicas tomadas

1. **.NET 9 como versión base:**  
   Se utiliza la versión más reciente compatible con C# 13 y EF Core 9 para optimizar rendimiento y modernizar la estructura del proyecto.

2. **Entity Framework Core:**  
   Se emplea para la comunicación con SQL Server, mapeando entidades y permitiendo ejecutar procedimientos almacenados (SP).

3. **Autenticación JWT:**  
   Implementada mediante `Microsoft.IdentityModel.Tokens` y `System.IdentityModel.Tokens.Jwt`.  
   Permite el acceso seguro a los endpoints mediante un token Bearer.

4. **Logs y manejo de errores:**  
   Todos los controladores cuentan con registro de logs (`ILogger`) y control centralizado de excepciones.

5. **Uso de procedimientos almacenados (SP_TASK):**  
   Se decidió manejar consultas complejas y filtros dinámicos mediante SQL Server SPs, mejorando la performance y escalabilidad.

6. **Swagger:**  
   Documentación automática habilitada en entorno de desarrollo (`/swagger`).

7. **Buenas prácticas:**  
   - Inyección de dependencias en `Program.cs`.  
   - Validaciones en DTOs y entidades.  
   - Separación clara entre lógica, infraestructura y presentación.

---

## 🧱 Base de datos

Los scripts SQL se encuentran en:

---
/WebApi_OnOff_ToDoList.Application/Database/
│
├── DDL/
│ ├── create_tables.sql → Definición de tablas (TBL_USER, TBL_TASK, TBL_STATUSTASK)
│
├── DML/
│ ├── insert_base_data.sql → Datos iniciales (usuarios, estados, tareas de ejemplo)
│
└── StoredProcedures/
├── SP_TASK.sql → Procedimiento para filtros, métricas y paginación
---
**Tablas principales:**
- `TBL_USER` → Usuarios del sistema  
- `TBL_TASK` → Tareas asignadas  
- `TBL_STATUSTASK` → Estados de tareas (Bloqueado, Por hacer, En curso, QA, Listo)

---

## 🚀 Ejecución del proyecto

### 🔹 Requisitos previos
- .NET SDK 9.0
- SQL Server 2019 o superior
- Visual Studio 2022 o VS Code
- Swagger o Postman (para pruebas)
- (Opcional) Angular 19 para consumir la API

### 🔹 Pasos para ejecutar

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/OnOff-ToDoList/WebApi_OnOff_ToDoList.git
   cd WebApi_OnOff_ToDoLis

---
2. Crear la base de datos:
> Ejecuta los scripts DDL y DML ubicados en Application/Database.
> Verifica que las tablas y el SP estén creados correctamente.

3. Configurar la cadena de conexión en appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=OnOff_ToDoList;Trusted_Connection=True;TrustServerCertificate=True;"
}


4. Ejecutar el proyecto:
```
dotnet build
dotnet run
```

5.  Acceder al Swagger:

https://localhost:7266/swagger
