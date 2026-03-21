using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace FCG.Users.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class Adjust_Audit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_audit_trails",
                table: "audit_trails");

            migrationBuilder.RenameTable(
                name: "audit_trails",
                newName: "AuditTrail");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AuditTrail",
                newName: "PerformedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_audit_trails_EntityName",
                table: "AuditTrail",
                newName: "IX_AuditTrail_EntityName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditTrail",
                table: "AuditTrail",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditTrail",
                table: "AuditTrail");

            migrationBuilder.RenameTable(
                name: "AuditTrail",
                newName: "audit_trails");

            migrationBuilder.RenameColumn(
                name: "PerformedByUserId",
                table: "audit_trails",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditTrail_EntityName",
                table: "audit_trails",
                newName: "IX_audit_trails_EntityName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_audit_trails",
                table: "audit_trails",
                column: "Id");
        }
    }
}
