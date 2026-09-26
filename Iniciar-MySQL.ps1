param(
    [string]$Usuario = "root",
    [string]$Servidor = "127.0.0.1",
    [int]$Puerto = 3306,
    [string]$BaseDatos = "clientes_api",
    [switch]$OmitirRestauracion,
    [switch]$ConexionLocalSinTls,
    [switch]$AplicarMigraciones
)
$ErrorActionPreference = "Stop"
if ($ConexionLocalSinTls -and $Servidor -notin @("127.0.0.1", "localhost", "::1")) {
    throw "ConexionLocalSinTls solo está permitido para el servidor local."
}
Push-Location $PSScriptRoot
$previousConnection = $env:ConnectionStrings__Clientes
try {
    if (-not $OmitirRestauracion) {
        dotnet restore Clientes.slnx
        if ($LASTEXITCODE -ne 0) { throw "No se pudieron restaurar los paquetes." }
        dotnet tool restore
        if ($LASTEXITCODE -ne 0) { throw "No se pudo restaurar dotnet-ef." }
    }
    $password = Read-Host "Contraseña de MySQL para $Usuario (no se guardará)" -AsSecureString
    $credential = [System.Net.NetworkCredential]::new("", $password)
    $connection = [System.Data.Common.DbConnectionStringBuilder]::new()
    $connection["Server"] = $Servidor
    $connection["Port"] = [string]$Puerto
    $connection["Database"] = $BaseDatos
    $connection["User ID"] = $Usuario
    $connection["Password"] = $credential.Password
    $connection["SslMode"] = if ($ConexionLocalSinTls) { "Disabled" } else { "Preferred" }
    $connection["CharSet"] = "utf8mb4"
    $env:ConnectionStrings__Clientes = $connection.ConnectionString
    $connection.Clear()
    $credential = $null
    $password.Dispose()
    dotnet build Clientes.slnx --no-restore
    if ($LASTEXITCODE -ne 0) { throw "No se pudo compilar la API." }
    if ($AplicarMigraciones) {
        dotnet ef database update --project Clientes.Api --no-build
        if ($LASTEXITCODE -ne 0) { throw "No se pudo aplicar la migración en MySQL. Comprueba usuario, contraseña y permisos sobre $BaseDatos." }
    }
    dotnet run --project Clientes.Api --no-build --launch-profile http --urls http://localhost:5080
    if ($LASTEXITCODE -ne 0) { throw "La API terminó con error." }
}
finally {
    $env:ConnectionStrings__Clientes = $previousConnection
    Pop-Location
}
