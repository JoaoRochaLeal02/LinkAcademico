using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkAcademico.Migrations
{
    /// <inheritdoc />
    public partial class RelacionamentoUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Estudantes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Empresas",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Estudantes_UserId",
                table: "Estudantes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_UserId",
                table: "Empresas",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Empresas_AspNetUsers_UserId",
                table: "Empresas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Estudantes_AspNetUsers_UserId",
                table: "Estudantes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empresas_AspNetUsers_UserId",
                table: "Empresas");

            migrationBuilder.DropForeignKey(
                name: "FK_Estudantes_AspNetUsers_UserId",
                table: "Estudantes");

            migrationBuilder.DropIndex(
                name: "IX_Estudantes_UserId",
                table: "Estudantes");

            migrationBuilder.DropIndex(
                name: "IX_Empresas_UserId",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Estudantes");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Empresas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
