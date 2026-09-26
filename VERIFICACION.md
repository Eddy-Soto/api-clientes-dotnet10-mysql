# Verificación de la adaptación a MySQL

## Swagger UI y procedimiento del tutorial

- Swagger UI integrado en /swagger y verificado en el navegador: aparecen GET, GET por ID, POST, PUT y DELETE.
- Consulta ejecutada con Try it out / Execute: respuesta 200 y cuerpo JSON desde MySQL.
- Las 14 comprobaciones HTTP del CRUD se volvieron a ejecutar correctamente tras integrar Swagger.
- Iniciar-MySQL.ps1 ahora usa la tabla existente por defecto. Solo aplica migraciones al solicitar -AplicarMigraciones.
- Generador oficial 10.0.2 ejecutado en una copia: produjo ClientesController.cs usando el contexto MySql.EntityFrameworkCore. El controlador generado compiló correctamente. Su salida exacta está en tutorial/ClientesController.generado.cs.txt.
- El controlador activo conserva las validaciones previas; no se presenta como salida sin cambios del generador.
- El generador necesitó Microsoft.EntityFrameworkCore.Tools y un proveedor admitido por su opción dbProvider. Se utilizó SQLite únicamente como dependencia temporal de la copia; la salida confirmó el uso del contexto MySQL existente.

## Base de datos

- Instalación detectada: MySQL Server 8.0.46; servicio MySQL80 en ejecución.
- Conexión autenticada como root en 127.0.0.1:3306 con las credenciales autorizadas de la conversación anterior. Las contraseñas no se guardaron en los entregables.
- Proyecto net10.0 con MySql.EntityFrameworkCore 10.0.9. El paquete contiene soporte específico para EF Core 10.
- UseMySQL reemplaza UseSqlite; conflictos de CUI usan el error MySQL 1062.
- Compilación correcta: cero errores y cero advertencias.
- Migraciones InitialMySql y ConvertirFechaMySql aplicadas en clientes_api y SQL exportado para revisión.
- Tabla Clientes verificada directamente en MySQL: motor InnoDB y utf8mb4_0900_ai_ci.
- tests/smoke.ps1: 14 comprobaciones HTTP aprobadas contra MySQL; CRUD completo, Location, unicidad de CUI, validaciones y respuestas 404.
- Conversión DateOnly/DateTime aplicada para corregir la lectura de fecha de nacimiento con MySql.Data.
- Modelo y snapshot de migración sincronizados.
- Registros temporales eliminados; la tabla quedó vacía y lista para ingresar clientes.

Se creó únicamente la base clientes_api de esta solución; no se modificaron bases anteriores ni usuarios. El script Iniciar-MySQL.ps1 permite introducir la contraseña localmente sin guardarla.

En este entorno Windows no permitió negociar TLS desde .NET (SEC_E_NO_CREDENTIALS). La conexión de prueba usó SslMode=Disabled exclusivamente para 127.0.0.1. No se cambió la configuración TLS del servidor ni el valor Preferred predeterminado de la API. El script incluye una opción explícita para repetir esta conexión local.
