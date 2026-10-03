using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkAcademico.Migrations
{
    /// <inheritdoc />
    public partial class AderenciaHackathonAtividadesEgressos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoAtividade",
                table: "Experiencias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EhEgressoEmpreendedor",
                table: "Empresas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoAtividade",
                table: "Experiencias");

            migrationBuilder.DropColumn(
                name: "EhEgressoEmpreendedor",
                table: "Empresas");
        }
    }
}
