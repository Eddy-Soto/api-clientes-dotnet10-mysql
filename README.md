# API de clientes .NET 10 con Azure SQL

API REST adaptada de MySQL a Azure SQL Database. Blazor consume la API mediante HTTP; la conexión SQL pertenece exclusivamente a la API.

## Base de datos
- Base utilizada: db_clientes.
- Región: France Central, permitida por la política de la suscripción utilizada.
- Servidor: sql-clientes-eddyfr-2026.database.windows.net.
- Tabla: dbo.clientes.
- Oferta gratuita con facturación por excedentes deshabilitada.
- Ejecutar Esquema-AzureSQL.sql en la base de destino para una instalación nueva.
- Autorizar la IP pública de la computadora en el firewall del servidor.
- Las migraciones incluidas son históricas de MySQL: no ejecutar database update contra Azure SQL. Este proyecto utiliza el esquema SQL adjunto.
- El proveedor MySQL permanece como dependencia para compilar las migraciones históricas.

## Configuración local
Requisitos: SDK .NET 10 y acceso de red al servidor SQL.

Desde la carpeta Clientes.Api, guardar la cadena en Secretos de usuario:
ConnectionStrings:ConexionSQLAzure

Formato (completar localmente; nunca publicar la contraseña):
Server=tcp:SERVIDOR.database.windows.net,1433;Initial Catalog=db_clientes;User ID=USUARIO;Password=CONTRASEÑA;Encrypt=True;TrustServerCertificate=False;Connection Timeout=120;

Los Secretos de usuario quedan fuera del repositorio, pero no están cifrados en disco.
La aplicación debe iniciarse en Development para cargarlos automáticamente.

En PowerShell:
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project ./Clientes.Api.csproj --urls http://localhost:5092

Swagger: http://localhost:5092/swagger
Clientes: http://localhost:5092/api/clientes

El frontend Blazor usa Api:BaseUrl = http://localhost:5092/
Puede iniciarse en http://localhost:5080.
La base está en Azure; API y frontend aún se ejecutan localmente. App Service no está desplegado.

## Operaciones
GET /api/clientes
GET /api/clientes/{id}
POST /api/clientes
PUT /api/clientes/{id}
DELETE /api/clientes/{id}

El cuerpo de POST y PUT contiene cui, nit, nombres, apellidos, direccion, telefono y fecha_Nacimiento.
Las validaciones actuales de entrada siguen exigiendo todos los campos, aunque el esquema admite nulos en algunos.
Longitudes: CUI 13, NIT 15, nombres y apellidos 60, dirección 150, teléfono 12.
La tabla no impone unicidad de CUI. Ocultar el identificador en Blazor no constituye autorización.
No se incorpora autenticación de usuarios en esta adaptación.

## Verificación realizada
- CRUD completo comprobado por el usuario con Swagger y Blazor contra Azure SQL.
- Creación: 201. Actualización: 204 y lectura del valor modificado.
- Eliminación y consulta posterior de lista vacía.
- Confirmación y cancelación de eliminación desde Blazor.
- Compilación correcta y auditoría NuGet sin vulnerabilidades reportadas tras retirar CodeGeneration.Design y fijar System.Security.Cryptography.Pkcs 9.0.21.
- Esta copia de entrega requiere su propia verificación de compilación.
