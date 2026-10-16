using Clientes.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Api.Data;

public class ClientesDbContext(
    DbContextOptions<ClientesDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cliente = modelBuilder.Entity<Cliente>();

        cliente.ToTable("clientes", "dbo");
        cliente.HasKey(x => x.Id_cliente);

        cliente.Property(x => x.Id_cliente)
            .HasColumnName("id_cliente")
            .ValueGeneratedOnAdd();

        cliente.Property(x => x.CUI)
            .HasColumnName("cui")
            .HasMaxLength(13)
            .IsUnicode(false)
            .IsRequired(false);

        cliente.Property(x => x.NIT)
            .HasColumnName("nit")
            .HasMaxLength(15)
            .IsUnicode(false)
            .IsRequired();

        cliente.Property(x => x.Nombres)
            .HasColumnName("nombres")
            .HasMaxLength(60)
            .IsUnicode(false)
            .IsRequired();

        cliente.Property(x => x.Apellidos)
            .HasColumnName("apellidos")
            .HasMaxLength(60)
            .IsUnicode(false)
            .IsRequired();

        cliente.Property(x => x.Direccion)
            .HasColumnName("direccion")
            .HasMaxLength(150)
            .IsUnicode(false)
            .IsRequired(false);

        cliente.Property(x => x.Telefono)
            .HasColumnName("telefono")
            .HasMaxLength(12)
            .IsUnicode(false)
            .IsRequired(false);

        cliente.Property(x => x.Fecha_Nacimiento)
            .HasColumnName("fecha_nacimiento")
            .HasColumnType("date")
            .IsRequired(false);
    }
}