# API REST de clientes — .NET 10

API REST para administrar clientes con ASP.NET Core **.NET 10**, Entity Framework Core 10 y **MySQL Server 8.0**. Incluye Swagger UI, CRUD asíncrono, validaciones y pruebas HTTP.

Este repositorio contiene el código para ejecutar la API en tu propio equipo. El enlace de GitHub no aloja la API ni da acceso a la base de datos del autor. No incluye contraseñas ni datos personales reales.

## Primera ejecución en otro equipo

1. Instala el SDK de .NET 10, MySQL Server 8.0 y PowerShell 7. Inicia el servicio de MySQL.
2. Descarga el ZIP del repositorio desde Code → Download ZIP y extráelo, o clónalo con Git.
3. Abre PowerShell en la carpeta que contiene Clientes.slnx.
4. Ejecuta:

```powershell
pwsh -File ./Iniciar-MySQL.ps1 -Usuario root -AplicarMigraciones
```

Introduce **tu propia contraseña de MySQL** cuando se solicite. Puedes indicar otro usuario con permisos para crear la base y las tablas. La opción -AplicarMigraciones crea clientes_api y su esquema en tu servidor; solo se necesita al instalar o actualizar el esquema. Si tu servidor usa otro puerto, añade -Puerto 3307 (o el valor correspondiente).

5. Abre http://localhost:5080/swagger para probar el CRUD.
6. En otra terminal, desde la misma carpeta, ejecuta:

```powershell
pwsh -File ./tests/smoke.ps1
```

Resultado esperado: **OK: 14 comprobaciones HTTP**. La prueba crea y elimina datos ficticios propios. Para detener la API, vuelve a su terminal y presiona Ctrl+C.

La contraseña solo se utiliza en la sesión de ejecución. No la agregues a appsettings.json ni al repositorio.

## Ejecutar

Requisitos: SDK de .NET 10, PowerShell 7, MySQL en ejecución, un usuario MySQL con contraseña y permisos sobre la base clientes_api, e Internet para restaurar paquetes de NuGet. Abrir una terminal en esta carpeta (la que contiene Clientes.slnx):

```powershell
pwsh -File .\Iniciar-MySQL.ps1 -Usuario root
```

La conexión se verificó con el usuario root existente. El script pide la contraseña de forma oculta, la transmite por una variable de entorno del proceso y no la guarda en archivos. Restaura dependencias, compila y arranca la API sobre la tabla existente. No aplica migraciones por defecto. La base clientes_api y su tabla Clientes ya están creadas en este ordenador.

La configuración base está en Clientes.Api/appsettings.json, sin contraseña. El script reemplaza la cadena completa mediante ConnectionStrings__Clientes. Para otro usuario o servidor se pueden pasar -Usuario, -Servidor, -Puerto y -BaseDatos. La conexión normal usa SslMode=Preferred.

En el entorno de ejecución de Codex, Windows rechazó la negociación TLS (SEC_E_NO_CREDENTIALS). Las pruebas se realizaron exclusivamente en 127.0.0.1 con TLS desactivado para esa conexión, sin cambiar el servidor. Si aparece ese mismo error al ejecutar localmente, usar:

```powershell
pwsh -File .\Iniciar-MySQL.ps1 -Usuario root -ConexionLocalSinTls
```

Este parámetro solo permite direcciones de bucle local.

La migración InitialMySql documenta la creación original de Clientes. Esquema-MySQL.sql muestra el SQL generado por EF. En otro equipo con una base nueva, pasar -AplicarMigraciones al script de inicio para crear el esquema. En este ordenador la tabla ya existe y no se necesita esa opción. Los datos del antiguo archivo SQLite no se transfieren a MySQL automáticamente.

**Swagger UI:** http://localhost:5080/swagger. Abrir una operación, pulsar **Try it out**, completar el ID o JSON cuando corresponda y pulsar **Execute**. Las cinco operaciones están disponibles. Para POST y PUT usar el ejemplo válido de abajo; los valores automáticos como "string" no cumplen la validación del CUI.

API: http://localhost:5080/api/clientes. Especificación OpenAPI: http://localhost:5080/openapi/v1.json. Swagger UI y OpenAPI solo se publican en Development (el perfil http ya lo configura). También se pueden usar los ejemplos de Clientes.http.

## Operaciones

| Método | Ruta | Resultado |
|---|---|---|
| GET | /api/clientes | 200, lista completa (vacía si no hay registros) |
| GET | /api/clientes/{id} | 200 con cliente, o 404 |
| POST | /api/clientes | 201 con cliente y cabecera Location |
| PUT | /api/clientes/{id} | 204, o 404 |
| DELETE | /api/clientes/{id} | 204, o 404 |

POST y PUT reciben los mismos campos. Id_cliente es una clave entera autogenerada y no se recibe en el cuerpo; PUT identifica al cliente mediante la ruta y reemplaza todos sus campos editables.

```json
{
  "cui": "1234567890101",
  "nit": "1234567-8",
  "nombres": "Ana",
  "apellidos": "López",
  "direccion": "Ciudad de Guatemala",
  "telefono": "+502 5555 1234",
  "fecha_Nacimiento": "1995-05-20"
}
```

La respuesta agrega id_cliente. Los nombres C# Direccion y Telefono corresponden a Dirección y Teléfono. CUI, NIT y teléfono son cadenas para conservar ceros, letras y signos. La fecha usa el formato ISO YYYY-MM-DD.

## Validaciones y decisiones

- Todos los campos son obligatorios; fecha no futura.
- CUI: exactamente 13 dígitos y único en la base de datos. Esta regla asume un CUI guatemalteco y no verifica su autenticidad.
- NIT: máximo 20 caracteres; no se exige unicidad ni se valida contra registros tributarios.
- Nombres y apellidos: máximo 100 caracteres cada uno; dirección: 250; teléfono: 25 y formato telefónico.
- Cuerpo inválido: 400 con detalles de validación. CUI duplicado: 409, incluso ante inserciones simultáneas por el índice único.
- Operaciones asíncronas y cancelación de solicitudes; consultas de lectura sin seguimiento de EF.

## Estructura

- Clientes.Api/Models/Cliente.cs: entidad persistida.
- Clientes.Api/Contracts/ClienteRequest.cs: datos de entrada y validaciones.
- Clientes.Api/Data/ClientesDbContext.cs: tabla, clave e índice.
- Clientes.Api/Controllers/ClientesController.cs: cinco operaciones REST.
- Clientes.Api/Migrations/: esquema versionado de EF Core.
- Iniciar-MySQL.ps1: configuración temporal de credenciales y arranque sobre tabla existente; migración opcional.
- tutorial/GENERACION.md: procedimiento comprobado del generador de controladores.
- tutorial/ClientesController.generado.cs.txt: salida real del generador, conservada como referencia.
- Esquema-MySQL.sql: esquema MySQL generado por EF.
- Clientes.http: ejemplos de solicitudes.
- tests/smoke.ps1: prueba HTTP del CRUD y validaciones contra una API en ejecución.

Para generar otra migración después de modificar el modelo:

```powershell
dotnet ef migrations add NombreDelCambio --project Clientes.Api
```

Con la API en ejecución, ejecutar la prueba desde otra terminal PowerShell 7:

```powershell
pwsh -File tests/smoke.ps1
```

La prueba crea un cliente con CUI aleatorio, verifica los códigos HTTP y los datos devueltos, y elimina su registro al finalizar.

Estado verificado: compilación correcta, migraciones aplicadas y 14 comprobaciones HTTP aprobadas contra MySQL Server 8.0.46. Se verifican también los datos devueltos, incluida la fecha de nacimiento. El modelo utiliza una conversión DateOnly/DateTime para leer el tipo SQL date con MySql.Data. Los registros temporales de prueba fueron eliminados.

Esta entrega cubre el CRUD solicitado y se ejecuta localmente. Antes de exponer datos personales en un despliegue público, configurar autenticación, autorización y HTTPS.

Referencia de implementación: [tutorial oficial de APIs con controladores en ASP.NET Core 10](https://learn.microsoft.com/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0). Se revisó la transcripción completa del video proporcionada por el usuario. La equivalencia y el procedimiento del generador se explican en tutorial/GENERACION.md.
