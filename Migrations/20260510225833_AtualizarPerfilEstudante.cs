using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkAcademico.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarPerfilEstudante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Estudantes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Estudantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FotoPerfil",
                table: "Estudantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GitHub",
                table: "Estudantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Habilidades",
                table: "Estudantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LinkedIn",
                table: "Estudantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Periodo",
                table: "Estudantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Portfolio",
                table: "Estudantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Estudantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Estudantes");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Estudantes");

            migrationBuilder.DropColumn(
                name: "FotoPerfil",
                table: "Estudantes");

            migrationBuilder.DropColumn(
                name: "GitHub",
                table: "Estudantes");

            migrationBuilder.DropColumn(
                name: "Habilidades",
                table: "Estudantes");

            migrationBuilder.DropColumn(
                name: "LinkedIn",
                table: "Estudantes");

            migrationBuilder.DropColumn(
                name: "Periodo",
                table: "Estudantes");

            migrationBuilder.DropColumn(
                name: "Portfolio",
                table: "Estudantes");

            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Estudantes");
        }
    }
}
