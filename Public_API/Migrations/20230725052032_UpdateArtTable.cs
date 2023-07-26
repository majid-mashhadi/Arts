using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Public_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateArtTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageSource",
                table: "Arts",
                newName: "ArtSource");

            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "Arts",
                newName: "ArtName");

            migrationBuilder.AlterColumn<string>(
                name: "ArtId",
                table: "Arts",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "CanvasHeight",
                table: "Arts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CanvasWidth",
                table: "Arts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Arts",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanvasHeight",
                table: "Arts");

            migrationBuilder.DropColumn(
                name: "CanvasWidth",
                table: "Arts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Arts");

            migrationBuilder.RenameColumn(
                name: "ArtSource",
                table: "Arts",
                newName: "ImageSource");

            migrationBuilder.RenameColumn(
                name: "ArtName",
                table: "Arts",
                newName: "ImageName");

            migrationBuilder.AlterColumn<int>(
                name: "ArtId",
                table: "Arts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
