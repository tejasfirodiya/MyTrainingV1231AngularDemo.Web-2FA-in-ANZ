using Microsoft.EntityFrameworkCore.Migrations;

namespace MyTrainingV1231AngularDemo.Migrations
{
    public partial class Add_Description_To_Binary_Object : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AppBinaryObjects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AppBinaryObjects");
        }
    }
}
