using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyTrainingV1231AngularDemo.Migrations
{
    public partial class Upgraded_To_IdentityServer_v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConsumedTime",
                table: "AbpPersistedGrants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AbpPersistedGrants",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "AbpPersistedGrants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpPersistedGrants_Expiration",
                table: "AbpPersistedGrants",
                column: "Expiration");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPersistedGrants_SubjectId_SessionId_Type",
                table: "AbpPersistedGrants",
                columns: new[] { "SubjectId", "SessionId", "Type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AbpPersistedGrants_Expiration",
                table: "AbpPersistedGrants");

            migrationBuilder.DropIndex(
                name: "IX_AbpPersistedGrants_SubjectId_SessionId_Type",
                table: "AbpPersistedGrants");

            migrationBuilder.DropColumn(
                name: "ConsumedTime",
                table: "AbpPersistedGrants");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AbpPersistedGrants");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "AbpPersistedGrants");
        }
    }
}
