using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTrainingV1231AngularDemo.Migrations
{
    public partial class Add_Authenticator_Recovery_Code : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecoveryCode",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecoveryCode",
                table: "AbpUsers");
        }
    }
}
