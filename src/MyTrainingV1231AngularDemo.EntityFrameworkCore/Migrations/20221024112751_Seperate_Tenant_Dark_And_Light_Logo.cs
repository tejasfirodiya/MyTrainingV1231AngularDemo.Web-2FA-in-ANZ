using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTrainingV1231AngularDemo.Migrations
{
    public partial class Seperate_Tenant_Dark_And_Light_Logo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogoId",
                table: "AbpTenants",
                newName: "LightLogoId");

            migrationBuilder.RenameColumn(
                name: "LogoFileType",
                table: "AbpTenants",
                newName: "LightLogoFileType");

            migrationBuilder.AddColumn<string>(
                name: "DarkLogoFileType",
                table: "AbpTenants",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DarkLogoId",
                table: "AbpTenants",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DarkLogoFileType",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "DarkLogoId",
                table: "AbpTenants");

            migrationBuilder.RenameColumn(
                name: "LightLogoId",
                table: "AbpTenants",
                newName: "LogoId");

            migrationBuilder.RenameColumn(
                name: "LightLogoFileType",
                table: "AbpTenants",
                newName: "LogoFileType");
        }
    }
}
