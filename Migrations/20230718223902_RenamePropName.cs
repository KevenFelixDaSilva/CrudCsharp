using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CursoApiC_.Migrations
{
    /// <inheritdoc />
    public partial class RenamePropName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "Products",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Products",
                newName: "name");
        }
    }
}
