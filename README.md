# 🛡️ SupplierGuard - Plataforma de Debida Diligencia de Proveedores

Un sistema integral de gestión de proveedores con capacidades de screening en listas de alto riesgo, construido con .NET 10 y principios de Clean Architecture.

## 📋 Tabla de Contenidos

- [Descripción General](#descripción-general)
- [Características](#características)
- [Arquitectura](#arquitectura)
- [Prerequisitos](#prerequisitos)
- [Instalación](#instalación)
- [Configuración de Base de Datos](#configuración-de-base-de-datos)
- [Ejecutar la Aplicación](#ejecutar-la-aplicación)
- [Documentación de la API](#documentación-de-la-api)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Tecnologías Utilizadas](#tecnologías-utilizadas)

---

## 🎯 Descripción General

SupplierGuard es una aplicación web diseñada para facilitar los procesos de debida diligencia de proveedores. Permite a los usuarios gestionar información de proveedores y realizar screening automático contra listas de alto riesgo (OFAC, World Bank, Offshore Leaks).

Este proyecto implementa:
- ✅ Clean Architecture
- ✅ Domain-Driven Design (DDD)
- ✅ Patrón CQRS con MediatR
- ✅ Patrón Repository con Entity Framework Core
- ✅ Value Objects para validación de dominio
- ✅ Validación integral de entradas con FluentValidation
- ✅ Manejo global de excepciones
- ✅ Auditoría automática (CreatedAt, LastModifiedAt)

---

## ✨ Características

### Gestión de Proveedores (CRUD)
- Crear, leer, actualizar y eliminar proveedores
- 9 campos requeridos por proveedor (Razón Social, Nombre Comercial, RUT, etc.)
- Validación automática de todos los campos de entrada
- Restricción de RUT único (11 dígitos)
- Tracking automático de fechas

### Búsqueda y Filtrado Avanzado
- Búsqueda por: Razón Social, Nombre Comercial, RUT, Email
- Filtrar por: País, Rango de Facturación Anual
- Ordenar por: Cualquier campo (ascendente/descendente)
- Soporte de paginación

### Integración de Screening
- Integración con API de Screening del Ejercicio 1
- Soporte para 3 fuentes de datos: OFAC, World Bank, Offshore Leaks
- Selección personalizable de fuentes
- Detección automática de riesgo (propiedad IsHighRisk)
- Política de reintentos con backoff exponencial
- Manejo de límite de tasa (rate limit)

### Validación de Datos
- RUT: Exactamente 11 dígitos numéricos
- Email: Formato de email válido (máx. 100 caracteres)
- Teléfono: Formato de teléfono válido (7-20 caracteres)
- Sitio Web: Formato de URL válido (opcional)
- Facturación Anual: Decimal no negativo
- Todos los campos: Restricciones de longitud apropiadas

---

## 🏗️ Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                     Capa de API                              │
│  (Controllers, Middleware, Configuración de Swagger)         │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                 Capa de Aplicación                           │
│  (Handlers CQRS, DTOs, Validators, Behaviors, Mappings)     │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│               Capa de Infraestructura                        │
│  (EF Core, Repositorios, APIs Externas, Servicios)          │
└────────────────────┬────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────┐
│                   Capa de Dominio                            │
│  (Entidades, Value Objects, Enums, Reglas de Negocio)       │
└─────────────────────────────────────────────────────────────┘
```

### Proyectos

1. **SupplierGuard** - Capa de API (ASP.NET Core Web API)
2. **SupplierGuard.Application** - Capa de Aplicación (CQRS, DTOs, Validators)
3. **SupplierGuard.Infrastructure** - Capa de Infraestructura (Base de Datos, APIs Externas)
4. **SupplierGuard.Domain** - Capa de Dominio (Entidades, Value Objects)

---

## ⚙️ Prerequisitos

Antes de ejecutar la aplicación, asegúrate de tener instalado lo siguiente:

- ✅ [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- ✅ [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB, Express o Completo)
- ✅ [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/) (opcional)
- ✅ [Git](https://git-scm.com/) (para clonar el repositorio)

### Verificar Instalación

```bash
# Verificar versión de .NET
dotnet --version
# Debería mostrar: 10.x.x

# Verificar SQL Server (ejemplo con LocalDB)
sqllocaldb info
# Debería listar las instancias disponibles
```

---

## 📥 Instalación

### 1. Clonar el Repositorio

```bash
# Navegar a tu carpeta de proyectos
cd C:\Projects

# Clonar el repositorio
git clone <url-del-repositorio>
cd Ejercicio2
```

### 2. Restaurar Paquetes NuGet

```bash
# Restaurar paquetes para todos los proyectos
dotnet restore
```

### 3. Configurar Connection String

Edita `SupplierGuard/appsettings.json` y actualiza el connection string según tu configuración de SQL Server:

#### Opción A: SQL Server LocalDB (Recomendado para desarrollo local)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SupplierGuardDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

#### Opción B: SQL Server Express

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SupplierGuardDb;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

#### Opción C: SQL Server con Credenciales

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SupplierGuardDb;User Id=tu_usuario;Password=tu_contraseña;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

### 4. Configurar API de Screening (Ejercicio 1)

Si tienes el API del Ejercicio 1 ejecutándose, actualiza la URL base en `appsettings.json`:

```json
{
  "ExternalApis": {
    "ScreeningApi": {
      "BaseUrl": "https://localhost:7172",
      "TimeoutSeconds": 60,
      "RetryCount": 3
    }
  }
}
```

---

## 🗄️ Configuración de Base de Datos

Tienes dos opciones para configurar la base de datos:

### Opción A: Usando Migraciones de Entity Framework (Recomendado)

#### 1. Instalar Herramientas de EF Core (si no están instaladas)

```bash
dotnet tool install --global dotnet-ef
# O actualizar si ya están instaladas
dotnet tool update --global dotnet-ef
```

#### 2. Crear Migración

```bash
# Navegar a la raíz de la solución (donde está el archivo .sln)
cd C:\Users\USER\Documents\EY_FullstackJr\Ejercicio2

# Crear la migración inicial
dotnet ef migrations add InitialCreate --project SupplierGuard.Infrastructure --startup-project SupplierGuard
```

#### 3. Aplicar Migración a la Base de Datos

```bash
# Crear/actualizar la base de datos
dotnet ef database update --project SupplierGuard.Infrastructure --startup-project SupplierGuard
```

#### 4. Verificar Creación de Base de Datos

- Abre **SQL Server Management Studio (SSMS)** o **SQL Server Object Explorer** en Visual Studio
- Conéctate a tu instancia de SQL Server
- Verifica que la base de datos `SupplierGuardDb` existe
- Verifica que la tabla `Suppliers` fue creada con todas las columnas e índices

#### 5. Datos de Prueba (Automático)

La aplicación automáticamente carga 12 proveedores de prueba en la primera ejecución si la base de datos está vacía.

Para volver a cargar manualmente:

```bash
# En Program.cs, descomenta la línea:
# await ApplicationDbContextSeed.ReseedDatabaseAsync(context);
```

---

### Opción B: Usando Script SQL

Si prefieres no usar migraciones, puedes ejecutar el script SQL proporcionado:

#### 1. Generar Script SQL desde Migraciones

```bash
# Generar script SQL completo
dotnet ef migrations script --output Database/CreateDatabase.sql --project SupplierGuard.Infrastructure --startup-project SupplierGuard
```

#### 2. Ejecutar Script SQL

- Abre **SQL Server Management Studio (SSMS)**
- Conéctate a tu instancia de SQL Server
- Abre el archivo generado `Database/CreateDatabase.sql`
- Ejecuta el script

---

## 🚀 Ejecutar la Aplicación

### Usando .NET CLI

```bash
# Navegar a la carpeta del proyecto API
cd SupplierGuard

# Ejecutar la aplicación
dotnet run
```

### Usando Visual Studio

1. Abre `Ejercicio2.sln` en Visual Studio 2022
2. Establece `SupplierGuard` como proyecto de inicio (clic derecho → Establecer como Proyecto de Inicio)
3. Presiona `F5` o haz clic en el botón "Ejecutar"

### Salida Esperada

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7252
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5252
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Puntos de Acceso

- **Swagger UI**: https://localhost:7252/swagger
- **URL Base de la API**: https://localhost:7252/api
- **Health Check**: https://localhost:7252/health

---

## 📚 Documentación de la API

### Endpoints de Proveedores

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/Suppliers` | Obtener proveedores con filtros y paginación |
| GET | `/api/Suppliers/all` | Obtener todos los proveedores sin paginación |
| GET | `/api/Suppliers/{id}` | Obtener proveedor por ID |
| POST | `/api/Suppliers` | Crear nuevo proveedor |
| PUT | `/api/Suppliers/{id}` | Actualizar proveedor existente |
| DELETE | `/api/Suppliers/{id}` | Eliminar proveedor |

### Endpoints de Screening

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/Screening/{supplierId}` | Realizar screening personalizado (seleccionar fuentes) |
| POST | `/api/Screening/{supplierId}/quick` | Realizar screening rápido (todas las fuentes) |

### Parámetros de Consulta (GET /api/Suppliers)

- `searchTerm` (string): Buscar en Razón Social, Nombre Comercial, RUT, Email
- `country` (string): Filtrar por país (ej: "Peru", "Chile")
- `minRevenue` (decimal): Facturación anual mínima
- `maxRevenue` (decimal): Facturación anual máxima
- `orderBy` (string): Campo de ordenamiento (LegalName, CommercialName, TaxId, Country, AnnualRevenue, CreatedAt, LastModifiedAt)
- `ascending` (bool): Orden (true = ascendente, false = descendente)
- `pageNumber` (int): Número de página (por defecto: 1)
- `pageSize` (int): Tamaño de página (por defecto: 10, máx: 100)

### Ejemplos de Solicitudes

#### Crear Proveedor

```bash
POST https://localhost:7252/api/Suppliers
Content-Type: application/json

{
  "legalName": "Acme Corporation S.A.",
  "commercialName": "ACME Corp",
  "taxId": "20123456789",
  "phoneNumber": "+51987654321",
  "email": "contacto@acmecorp.com.pe",
  "website": "https://www.acmecorp.com.pe",
  "physicalAddress": "Av. Javier Prado Este 5250, La Molina, Lima",
  "country": "Peru",
  "annualRevenue": 15000000.00
}
```

#### Buscar Proveedores

```bash
GET https://localhost:7252/api/Suppliers?searchTerm=acme&country=Peru&minRevenue=1000000&orderBy=AnnualRevenue&ascending=false&pageNumber=1&pageSize=10
```

#### Realizar Screening

```bash
POST https://localhost:7252/api/Screening/{supplierId}
Content-Type: application/json

{
  "sources": [1, 2, 3]
}
```

**Fuentes:**
- 1 = Offshore Leaks
- 2 = World Bank
- 3 = OFAC

### Formato de Respuesta

Todos los endpoints retornan una respuesta estandarizada:

```json
{
  "success": true,
  "message": "Operación completada exitosamente",
  "data": { ... },
  "errors": [],
  "timestamp": "2025-01-20T10:30:00Z"
}
```

---

## 📁 Estructura del Proyecto

```
Ejercicio2/
├── SupplierGuard/                      # 🌐 Capa de API
│   ├── Controllers/
│   │   ├── SuppliersController.cs
│   │   └── ScreeningController.cs
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── Models/
│   │   └── ApiResponse.cs
│   ├── appsettings.json
│   └── Program.cs
│
├── SupplierGuard.Application/          # 💼 Capa de Aplicación
│   ├── Common/
│   │   ├── Behaviors/
│   │   ├── Exceptions/
│   │   └── Models/
│   ├── Suppliers/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   └── DTOs/
│   ├── Screening/
│   │   ├── Commands/
│   │   └── DTOs/
│   └── DependencyInjection.cs
│
├── SupplierGuard.Infrastructure/       # 🗄️ Capa de Infraestructura
│   ├── Persistence/
│   │   ├── Configurations/
│   │   ├── Seeds/
│   │   └── ApplicationDbContext.cs
│   ├── Repositories/
│   │   └── SupplierRepository.cs
│   ├── ExternalApis/
│   │   ├── ScreeningApiClient.cs
│   │   └── Auth0TokenHandler.cs
│   ├── Services/
│   │   └── DateTimeService.cs
│   └── DependencyInjection.cs
│
└── SupplierGuard.Domain/               # 🎯 Capa de Dominio
    ├── Entities/
    │   └── Supplier.cs
    ├── ValueObjects/
    │   ├── TaxId.cs
    │   ├── Email.cs
    │   ├── PhoneNumber.cs
    │   ├── Website.cs
    │   ├── Address.cs
    │   ├── AnnualRevenue.cs
    │   └── CompanyName.cs
    ├── Enums/
    │   └── Country.cs
    ├── Repositories/
    │   └── ISupplierRepository.cs
    └── Exceptions/
        └── (Excepciones de dominio)
```

---

## 🛠️ Tecnologías Utilizadas

### Backend
- **.NET 10** - Último framework de .NET
- **ASP.NET Core** - Framework de Web API
- **Entity Framework Core 10.0** - ORM
- **SQL Server** - Base de datos
- **MediatR 13.1** - Implementación de CQRS
- **FluentValidation 12.1** - Validación de entradas
- **AutoMapper 12.0** - Mapeo de objetos
- **Polly 8.6.5** - Políticas de resiliencia y reintentos

### Patrones y Prácticas
- Clean Architecture
- Domain-Driven Design (DDD)
- CQRS (Command Query Responsibility Segregation)
- Patrón Repository
- Patrón Unit of Work
- Patrón Specification
- Value Objects
- Aggregate Roots

### Documentación
- **Swagger/OpenAPI** - Documentación interactiva de API

---

## 🧪 Probar la Aplicación

### 1. Usando Swagger UI

1. Navega a https://localhost:7252/swagger
2. Prueba el endpoint `/api/Suppliers/all` para ver los datos de prueba
3. Crea un nuevo proveedor usando el endpoint POST
4. Prueba el filtrado y paginación

### 2. Usando Postman

Una colección de Postman está disponible en la carpeta `Postman/`:
- Importa `SupplierGuard.postman_collection.json`
- Establece la variable `baseUrl` a `https://localhost:7252`
- Ejecuta las solicitudes en orden

### 3. Usando curl

```bash
# Obtener todos los proveedores
curl -X GET "https://localhost:7252/api/Suppliers/all" -k

# Obtener proveedor por ID
curl -X GET "https://localhost:7252/api/Suppliers/{id}" -k

# Crear proveedor
curl -X POST "https://localhost:7252/api/Suppliers" \
  -H "Content-Type: application/json" \
  -d '{
    "legalName": "Empresa de Prueba",
    "commercialName": "Prueba Co",
    "taxId": "12345678901",
    "phoneNumber": "+51999888777",
    "email": "test@company.com",
    "website": "https://www.testcompany.com",
    "physicalAddress": "123 Test St, Lima",
    "country": "Peru",
    "annualRevenue": 5000000
  }' -k
```

---

## 🐛 Solución de Problemas

### Problema: Falla la conexión a la base de datos

**Solución:**
1. Verifica que SQL Server esté ejecutándose: `sqllocaldb info`
2. Revisa el connection string en `appsettings.json`
3. Asegúrate de que la base de datos fue creada: `dotnet ef database update`

### Problema: Falla la migración

**Solución:**
1. Elimina la carpeta `Migrations/`
2. Recrea la migración: `dotnet ef migrations add InitialCreate --project SupplierGuard.Infrastructure --startup-project SupplierGuard`
3. Aplica la migración: `dotnet ef database update --project SupplierGuard.Infrastructure --startup-project SupplierGuard`

### Problema: Puerto ya en uso

**Solución:**
1. Cambia el puerto en `SupplierGuard/Properties/launchSettings.json`
2. Actualiza `applicationUrl` para usar puertos diferentes

### Problema: API de Screening no responde

**Solución:**
1. Verifica que el API del Ejercicio 1 esté ejecutándose
2. Revisa `ExternalApis:ScreeningApi:BaseUrl` en `appsettings.json`
3. Verifica la configuración del firewall

---

## 👨‍💻 Autor

**Diego Flores**


