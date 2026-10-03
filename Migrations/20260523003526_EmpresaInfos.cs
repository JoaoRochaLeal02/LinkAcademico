using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkAcademico.Migrations
{
    /// <inheritdoc />
    public partial class EmpresaInfos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Empresas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OQueBusca",
                table: "Empresas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tecnologias",
                table: "Empresas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "OQueBusca",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "Tecnologias",
                table: "Empresas");
        }
    }
}
