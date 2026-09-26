param([string]$BaseUrl = "http://localhost:5080")
$ErrorActionPreference = "Stop"
$client = [System.Net.Http.HttpClient]::new()
$ids = [System.Collections.Generic.List[int]]::new()
$checks = 0
function Request($method, $path, $body, $expected) {
    $message = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::new($method), "$BaseUrl$path")
    if ($null -ne $body) {
        $message.Content = [System.Net.Http.StringContent]::new(($body | ConvertTo-Json), [System.Text.Encoding]::UTF8, "application/json")
    }
    $response = $client.SendAsync($message).GetAwaiter().GetResult()
    $text = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
    if ([int]$response.StatusCode -ne $expected) { throw "$method $path esperaba $expected, obtuvo $([int]$response.StatusCode): $text" }
    $script:checks++
    $result = if ($text) { $text | ConvertFrom-Json } else { $null }
    if ($method -eq "POST" -and $expected -eq 201) {
        $script:ids.Add([int]$result.id_cliente)
        if (-not $response.Headers.Location) { throw "Falta Location" }
    }
    $response.Dispose()
    $message.Dispose()
    return $result
}
try {
    $cui = "9" + ([string](Get-Random -Minimum 100000 -Maximum 999999)) + ([string](Get-Random -Minimum 100000 -Maximum 999999))
    $body = @{ cui=$cui; nit="1234567-8"; nombres="Ana"; apellidos="López"; direccion="Guatemala"; telefono="+502 5555 1234"; fecha_Nacimiento="1995-05-20" }
    $created = Request POST "/api/clientes" $body 201
    $id = $created.id_cliente
    $found = Request GET "/api/clientes/$id" $null 200
    if ($found.cui -ne $cui -or $found.apellidos -ne "López") { throw "Datos no persistidos correctamente" }
    if ([string]$found.fecha_Nacimiento -ne "1995-05-20") { throw "Fecha de nacimiento no recuperada correctamente" }
    $all = @(Request GET "/api/clientes" $null 200)
    if ($id -notin $all.id_cliente) { throw "Cliente ausente del listado" }
    $null = Request POST "/api/clientes" $body 409
    $body.nombres = "Ana María"
    $null = Request PUT "/api/clientes/$id" $body 204
    $updated = Request GET "/api/clientes/$id" $null 200
    if ($updated.nombres -ne "Ana María") { throw "Actualización no persistida" }
    $body.cui = "123"
    $null = Request POST "/api/clientes" $body 400
    $body.cui = $cui
    $body.fecha_Nacimiento = "2999-01-01"
    $null = Request PUT "/api/clientes/$id" $body 400
    $body.fecha_Nacimiento = "1995-05-20"
    $body.nombres = " "
    $null = Request POST "/api/clientes" $body 400
    $body.nombres = "Ana"
    $null = Request POST "/api/clientes" @{} 400
    $null = Request DELETE "/api/clientes/$id" $null 204
    $null = Request GET "/api/clientes/$id" $null 404
    $null = Request PUT "/api/clientes/$id" $body 404
    $null = Request DELETE "/api/clientes/$id" $null 404
    Write-Output "OK: $checks comprobaciones HTTP; CRUD, Location, persistencia, duplicados y validación."
}
finally {
    foreach ($id in $ids) {
        $response = $client.DeleteAsync("$BaseUrl/api/clientes/$id").GetAwaiter().GetResult()
        $response.Dispose()
    }
    $client.Dispose()
}
