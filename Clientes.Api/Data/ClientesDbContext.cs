using Clientes.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Api.Data;

public class ClientesDbContext(DbContextOptions<ClientesDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cliente = modelBuilder.Entity<Cliente>();
        cliente.HasKey(x => x.Id_cliente);
        cliente.HasIndex(x => x.CUI).IsUnique();
        cliente.Property(x => x.CUI).HasMaxLength(13).IsRequired();
        cliente.Property(x => x.NIT).HasMaxLength(20).IsRequired();
        cliente.Property(x => x.Nombres).HasMaxLength(100).IsRequired();
        cliente.Property(x => x.Apellidos).HasMaxLength(100).IsRequired();
        cliente.Property(x => x.Direccion).HasMaxLength(250).IsRequired();
        cliente.Property(x => x.Telefono).HasMaxLength(25).IsRequired();
        // El lector de MySql.Data devuelve DATE como DateTime.
        cliente.Property(x => x.Fecha_Nacimiento)
            .HasConversion(value => value.ToDateTime(TimeOnly.MinValue),
                value => DateOnly.FromDateTime(value))
            .HasColumnType("date");
    }
}
