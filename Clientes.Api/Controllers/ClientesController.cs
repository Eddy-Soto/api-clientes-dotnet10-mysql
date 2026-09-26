using Clientes.Api.Contracts;
using Clientes.Api.Data;
using Clientes.Api.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController(ClientesDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Cliente>>> Get(CancellationToken ct) =>
        await db.Clientes.AsNoTracking().OrderBy(x => x.Id_cliente).ToListAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cliente>> GetById(int id, CancellationToken ct)
    {
        var cliente = await db.Clientes.AsNoTracking().SingleOrDefaultAsync(x => x.Id_cliente == id, ct);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
    public async Task<ActionResult<Cliente>> Post(ClienteRequest request, CancellationToken ct)
    {
        var cliente = new Cliente();
        Apply(request, cliente);
        db.Clientes.Add(cliente);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException ex) when (IsDuplicate(ex)) { return Duplicate(); }
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id_cliente }, cliente);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, ClienteRequest request, CancellationToken ct)
    {
        var cliente = await db.Clientes.FindAsync([id], ct);
        if (cliente is null) return NotFound();
        Apply(request, cliente);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException) { return NotFound(); }
        catch (DbUpdateException ex) when (IsDuplicate(ex)) { return Duplicate(); }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var cliente = await db.Clientes.FindAsync([id], ct);
        if (cliente is null) return NotFound();
        db.Clientes.Remove(cliente);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException) { return NotFound(); }
        return NoContent();
    }

    private ObjectResult Duplicate() => Problem(statusCode: 409, title: "Ya existe un cliente con ese CUI.");
    private static bool IsDuplicate(DbUpdateException ex) =>
        ex.InnerException is MySqlException { Number: 1062 };

    private static void Apply(ClienteRequest source, Cliente target)
    {
        target.CUI = source.CUI.Trim();
        target.NIT = source.NIT.Trim();
        target.Nombres = source.Nombres.Trim();
        target.Apellidos = source.Apellidos.Trim();
        target.Direccion = source.Direccion.Trim();
        target.Telefono = source.Telefono.Trim();
        target.Fecha_Nacimiento = source.Fecha_Nacimiento;
    }
}
