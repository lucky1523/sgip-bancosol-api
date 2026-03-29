# ⚙️ SGIP API - FinTech Backend (.NET)

Backend para la gestion de prestamos e inversiones, desarrollado bajo estandares de arquitectura limpia para el Banco Sol.

## 🛠️ Stack Técnico

* **Lenguaje:** C# / .NET 8.0
* **ORM:** Entity Framework Core
* **Base de Datos:** PostgreSQL
* **Documentación:** Swagger / OpenAPI

## 📑 Endpoints Principales

* `GET /api/Loans`: Listado de créditos.
* `POST /api/Loans`: Registro y creación de cronograma.
* `GET /api/Transactions`: Historial de movimientos financieros.

## 🚀 Configuración Local

1. **Base de Datos:**
   Asegurar que la ConnectionString en `appsettings.json` apunte a su instancia de PostgreSQL.

2. **Migraciones:**
   ```bash
   dotnet ef database update
   
3. **Ejecutar:**
   ```bash
   dotnet run

---
Prueba Tecnica - Banco Sol