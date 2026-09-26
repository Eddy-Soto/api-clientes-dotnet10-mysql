# Generación del controlador como en el tutorial

## Resultado comprobado

Sí es factible. Se ejecutó dotnet-aspnet-codegenerator 10.0.2 en una copia del proyecto y se obtuvo un controlador CRUD asíncrono. La salida confirmó:

```text
Using database provider 'MySql.EntityFrameworkCore'!
Added Controller : '\Controllers\ClientesController.cs'.
```

ClientesController.generado.cs.txt contiene el archivo exacto producido por el generador. Se conserva como texto para que no se compile un segundo controlador con las mismas rutas.

El controlador activo conserva las validaciones, DTO de entrada, conversión de fechas del contexto y manejo de CUI duplicado comprobados previamente. No se sustituyó por la plantilla básica: la generación por comando es el punto de partida del código, no una condición para que el CRUD funcione.

## Reproducir la generación

Hacer una copia de la solución completa en otra carpeta y abrir una terminal en esa copia. El comando con -f sobrescribe el controlador de esa copia.

```powershell
dotnet tool restore
cd Clientes.Api
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 10.0.11
dotnet build
dotnet aspnet-codegenerator controller -name ClientesController -m Clientes.Api.Models.Cliente -dc Clientes.Api.Data.ClientesDbContext -api -async -outDir Controllers -dbProvider sqlite -f --no-build
```

El generador 10.0.2 exige uno de los proveedores admitidos por su opción dbProvider y no incluye mysql entre ellos. Se agregó SQLite únicamente en la copia para satisfacer ese requisito. Al encontrar el contexto existente registrado con UseMySQL, el generador utilizó MySql.EntityFrameworkCore, tal como confirmó su salida. No se creó ni utilizó una base SQLite. El proyecto principal no incorpora ese paquete.

Microsoft.VisualStudio.Web.CodeGeneration.Design y Microsoft.EntityFrameworkCore.Tools están incluidos como herramientas de desarrollo. El manifiesto local fija la versión del generador.

La plantilla generada usa la entidad completa en PUT y comprueba el ID del cuerpo contra el de la URL. La API activa sigue recibiendo ClienteRequest y tomando el ID únicamente de la URL; esta diferencia se mantiene explícita.

## Conectar una tabla existente

La API se conecta ahora a clientes_api.Clientes ya existente en el MySQL local. Iniciar-MySQL.ps1 ya no ejecuta migraciones por defecto. Las migraciones históricas se conservan para documentar cómo se creó esa tabla y para instalar la solución en otro equipo.

Para crear el esquema en una instalación nueva, usar expresamente -AplicarMigraciones. Esta opción no es necesaria en este ordenador.

No se cambió a db_empresa.clientes: esa tabla pertenece al trabajo anterior y tiene longitudes, nulabilidad y restricciones diferentes. Conectarla también es factible, pero exigiría adaptar el modelo y las validaciones a su esquema, además de decidir que la API debe administrar esos registros anteriores. Esta entrega utiliza la tabla existente de la propia API.
