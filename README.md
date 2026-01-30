# Products Management API

API REST desarrollada en **ASP.NET Core** para la gestión de productos, implementando arquitectura limpia, control de concurrencia optimista, paginación, validaciones, logging y pruebas unitarias.

El proyecto fue construido como **challenge técnico**, priorizando claridad, mantenibilidad, buenas prácticas y preparación para futuras ampliaciones.

---

## 📌 Detalles de la API

- Estilo: **REST**
- Versionado: `/api/v1`
- Formato de intercambio: **JSON**
- Concurrencia: **Optimistic Concurrency Control** mediante campo `Version`
- Documentación interactiva: **Swagger / OpenAPI**
- Manejo global de errores mediante middleware

---

## ✅ Requisitos Previos

- .NET SDK **8.0** o superior
- Visual Studio 2022 / VS Code
- Git (opcional, para clonar el repositorio) link: https://github.com/davidaragon08/products-management-api.git

---

## 🧱 Arquitectura General

El proyecto sigue **Clean Architecture**, separando responsabilidades en capas bien definidas:

API
└── Application
└── Domain
└── Infrastructure

---


### Capas
- **API**: Controllers, middlewares, configuración HTTP.
- **Application**: Casos de uso, DTOs, contratos de servicios.
- **Domain**: Entidades y reglas de negocio puras.
- **Infrastructure**: Implementaciones de persistencia (in-memory).

---

## 🧩 Principales Patrones de Diseño Aplicados

- **Repository Pattern**
- **Dependency Injection**
- **DTO Pattern**
- **Middleware Pattern**
- **Optimistic Concurrency Control**
- **Separation of Concerns**
- **Strategy (ordenación allow-list)**

---

## 🧪 Pila Tecnológica

- **.NET 8**
- **ASP.NET Core Web API**
- **xUnit** (tests)
- **Moq** (mocking)
- **Swagger / OpenAPI**
- **Serilog** (logging)
- **CORS / HTTPS**
- **Mermaid** (documentación de flujo)

---

## 🔄 Flujo de Ejecución (Mermaid)

```mermaid
sequenceDiagram
    Client ->> API Controller: HTTP Request
    API Controller ->> Application Service: Caso de uso
    Application Service ->> Repository: Acceso a datos
    Repository ->> Application Service: Resultado
    Application Service ->> API Controller: DTO
    API Controller ->> Client: HTTP Response

---

🔌 Endpoints Disponibles

| Método | Endpoint                        | Descripción                |
| ------ | ------------------------------- | -------------------------- |
| GET    | /api/v1/products                | Lista paginada con filtros |
| GET    | /api/v1/products/{id}           | Obtener producto           |
| POST   | /api/v1/products                | Crear producto             |
| PUT    | /api/v1/products/{id}           | Reemplazo completo         |
| PATCH  | /api/v1/products/{id}           | Actualización parcial      |
| DELETE | /api/v1/products/{id}?version=X | Eliminar producto          |
| GET    | /health                         | Health check               |

---

📥 Ejemplos de Respuestas

Crear producto

POST /api/v1/products

{
  "name": "Teclado Mecánico",
  "price": 120.50,
  "quantity": 10
}


Respuesta 201 Created

{
  "id": "guid",
  "name": "Teclado Mecánico",
  "price": 120.50,
  "quantity": 10,
  "version": 1
}

Error de concurrencia

409 Conflict

{
  "status": 409,
  "error": "Conflicto de concurrencia: la versión actual es 3.",
  "traceId": "abc-123"
}

---

▶️ Ejecución y Pruebas

dotnet restore
dotnet build
dotnet run --project ProductsManagement.Api

Acceder a:

Swagger: https://localhost:{puerto}/swagger

Health: https://localhost:{puerto}/health

---

🧪 Ejecución de Tests

dotnet test -v normal

dotnet test
Incluye:

- Tests de dominio

- Tests de repositorio

- Tests de servicios de aplicación

- Validación de DTOs

---


🧬 Modelo Entidad-Relación

erDiagram
    PRODUCT {
        Guid Id
        string Name
        decimal Price
        int Quantity
        int Version
    }

---

🧠 Buenas Prácticas Aplicadas

- Clean Architecture

- Principios SOLID

- Validación en la frontera (DTOs)

- Control de concurrencia optimista

- Logs estructurados

- Código documentado (XML Comments)

- Controllers delgados

- Tests aislados por capa

- Código preparado para persistencia real (EF Core)

---

## 🔧 Principios SOLID Aplicados

El diseño de la solución aplica explícitamente varios principios **SOLID**, garantizando mantenibilidad, extensibilidad y bajo acoplamiento.

### ✅ Single Responsibility Principle (SRP)
Cada clase tiene una única responsabilidad bien definida:
- `ProductsController`: orquestación HTTP y códigos de estado.
- `ProductService`: lógica de negocio y casos de uso.
- `InMemoryProductRepository`: acceso a datos.
- `ExceptionMiddleware`: manejo centralizado de errores.

Esto facilita cambios aislados sin efectos colaterales.

---

### ✅ Open/Closed Principle (OCP)
La solución está **abierta a extensión y cerrada a modificación**:
- La persistencia puede cambiarse (EF Core, SQL Server, PostgreSQL, etc.) sin modificar la lógica de negocio.
- Basta con crear una nueva implementación de `IProductRepository`.

---

### ✅ Liskov Substitution Principle (LSP)
Las implementaciones concretas (`InMemoryProductRepository`) pueden sustituir a la abstracción (`IProductRepository`) sin alterar el comportamiento esperado por la capa Application.

---

### ✅ Interface Segregation Principle (ISP)
Las interfaces están diseñadas de forma específica y cohesiva:
- `IProductService` define únicamente los casos de uso necesarios.
- `IProductRepository` expone solo operaciones relacionadas a persistencia de productos.

No existen interfaces con métodos innecesarios.

---

### ✅ Dependency Inversion Principle (DIP)
Las capas de alto nivel dependen de **abstracciones**, no de implementaciones concretas:
- La capa Application depende de `IProductRepository`, no de su implementación.
- La inyección de dependencias se configura en el arranque de la aplicación.

Esto desacopla completamente la lógica de negocio de la infraestructura.

---

🛠️ Scripts Útiles

# Restaurar dependencias
dotnet restore

# Ejecutar API
dotnet run --project ProductsManagement.Api

# Ejecutar tests
dotnet test


---

🚀 Próximas Mejoras Recomendadas

- Implementar EF Core con SQL Server / PostgreSQL

- Agregar autenticación y autorización (JWT)

- Cacheo (Redis)

- Versionado avanzado de API

- Tests de integración

- Dockerización

- Rate limiting


---


👤 Autor

David Aragón
Software Engineer