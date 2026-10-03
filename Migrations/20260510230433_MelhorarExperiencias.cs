using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkAcademico.Migrations
{
    /// <inheritdoc />
    public partial class MelhorarExperiencias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CertificadoUrl",
                table: "Experiencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Competencias",
                table: "Experiencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Instituicao",
                table: "Experiencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificadoUrl",
                table: "Experiencias");

            migrationBuilder.DropColumn(
                name: "Competencias",
                table: "Experiencias");

            migrationBuilder.DropColumn(
                name: "Instituicao",
                table: "Experiencias");
        }
    }
}
