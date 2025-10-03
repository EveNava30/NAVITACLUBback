using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReservasNC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estatus",
                columns: table => new
                {
                    IdEstatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaUltimaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estatus", x => x.IdEstatus);
                });

            migrationBuilder.CreateTable(
                name: "Restaurante",
                columns: table => new
                {
                    IdRestaurante = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    FechaUltimaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurante", x => x.IdRestaurante);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    IdRole = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreRol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaUltimaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.IdRole);
                });

            migrationBuilder.CreateTable(
                name: "Mesa",
                columns: table => new
                {
                    IdMesa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroMesa = table.Column<int>(type: "int", nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    IdRestaurante = table.Column<int>(type: "int", nullable: false),
                    FechaUltimaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesa", x => x.IdMesa);
                    table.ForeignKey(
                        name: "FK_Mesa_Restaurante_IdRestaurante",
                        column: x => x.IdRestaurante,
                        principalTable: "Restaurante",
                        principalColumn: "IdRestaurante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdRole = table.Column<int>(type: "int", nullable: false),
                    FechaUltimaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUser);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol_IdRole",
                        column: x => x.IdRole,
                        principalTable: "Rol",
                        principalColumn: "IdRole",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reporte",
                columns: table => new
                {
                    IdReporte = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaUltimaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reporte", x => x.IdReporte);
                    table.ForeignKey(
                        name: "FK_Reporte_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reserva",
                columns: table => new
                {
                    IdReserva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdMesa = table.Column<int>(type: "int", nullable: false),
                    CodigoQR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaUltimaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reserva", x => x.IdReserva);
                    table.ForeignKey(
                        name: "FK_Reserva_Mesa_IdMesa",
                        column: x => x.IdMesa,
                        principalTable: "Mesa",
                        principalColumn: "IdMesa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reserva_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReservaEstatus",
                columns: table => new
                {
                    IdReservaEstatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdReserva = table.Column<int>(type: "int", nullable: false),
                    IdEstatus = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaUltimaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservaEstatus", x => x.IdReservaEstatus);
                    table.ForeignKey(
                        name: "FK_ReservaEstatus_Estatus_IdEstatus",
                        column: x => x.IdEstatus,
                        principalTable: "Estatus",
                        principalColumn: "IdEstatus",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservaEstatus_Reserva_IdReserva",
                        column: x => x.IdReserva,
                        principalTable: "Reserva",
                        principalColumn: "IdReserva",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Estatus",
                columns: new[] { "IdEstatus", "FechaUltimaModificacion", "Nombre" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reservado" },
                    { 2, new DateTime(2025, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Completado" },
                    { 3, new DateTime(2025, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cancelado" }
                });

            migrationBuilder.InsertData(
                table: "Rol",
                columns: new[] { "IdRole", "FechaUltimaModificacion", "NombreRol" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cliente" },
                    { 2, new DateTime(2025, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mesero" },
                    { 3, new DateTime(2025, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Administrador" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mesa_IdRestaurante",
                table: "Mesa",
                column: "IdRestaurante");

            migrationBuilder.CreateIndex(
                name: "IX_Reporte_IdUsuario",
                table: "Reporte",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_IdMesa",
                table: "Reserva",
                column: "IdMesa");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_IdUsuario",
                table: "Reserva",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ReservaEstatus_IdEstatus",
                table: "ReservaEstatus",
                column: "IdEstatus");

            migrationBuilder.CreateIndex(
                name: "IX_ReservaEstatus_IdReserva",
                table: "ReservaEstatus",
                column: "IdReserva");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdRole",
                table: "Usuario",
                column: "IdRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reporte");

            migrationBuilder.DropTable(
                name: "ReservaEstatus");

            migrationBuilder.DropTable(
                name: "Estatus");

            migrationBuilder.DropTable(
                name: "Reserva");

            migrationBuilder.DropTable(
                name: "Mesa");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Restaurante");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
