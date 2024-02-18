using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTrainingV1231AngularDemo.Migrations
{
    /// <inheritdoc />
    public partial class Add_Minimal_Logo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DarkLogoMinimalFileType",
                table: "AbpTenants",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DarkLogoMinimalId",
                table: "AbpTenants",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LightLogoMinimalFileType",
                table: "AbpTenants",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LightLogoMinimalId",
                table: "AbpTenants",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DarkLogoMinimalFileType",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "DarkLogoMinimalId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "LightLogoMinimalFileType",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "LightLogoMinimalId",
                table: "AbpTenants");
        }
    }
}
