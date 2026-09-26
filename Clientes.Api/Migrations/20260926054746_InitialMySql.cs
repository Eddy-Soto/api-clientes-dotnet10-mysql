using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Clientes.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialMySql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id_cliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CUI = table.Column<string>(type: "varchar(13)", maxLength: 13, nullable: false),
                    NIT = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Nombres = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    Telefono = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false),
                    Fecha_Nacimiento = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id_cliente);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CUI",
                table: "Clientes",
                column: "CUI",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
