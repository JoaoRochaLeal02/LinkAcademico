using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkAcademico.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarVagasCandidaturas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vagas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Requisitos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Local = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModeloTrabalho = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoVaga = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remuneracao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataLimite = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataPublicacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vagas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vagas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidaturasVagas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VagaId = table.Column<int>(type: "int", nullable: false),
                    EstudanteId = table.Column<int>(type: "int", nullable: false),
                    DataCandidatura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidaturasVagas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidaturasVagas_Estudantes_EstudanteId",
                        column: x => x.EstudanteId,
                        principalTable: "Estudantes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CandidaturasVagas_Vagas_VagaId",
                        column: x => x.VagaId,
                        principalTable: "Vagas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidaturasVagas_EstudanteId",
                table: "CandidaturasVagas",
                column: "EstudanteId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidaturasVagas_VagaId_EstudanteId",
                table: "CandidaturasVagas",
                columns: new[] { "VagaId", "EstudanteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vagas_EmpresaId",
                table: "Vagas",
                column: "EmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidaturasVagas");

            migrationBuilder.DropTable(
                name: "Vagas");
        }
    }
}
