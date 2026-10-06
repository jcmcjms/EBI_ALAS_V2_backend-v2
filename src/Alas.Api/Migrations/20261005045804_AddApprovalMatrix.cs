using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alas.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalMatrix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalAuthorities",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    AllowNew = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AllowRenewal = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaxSeverity = table.Column<int>(type: "int", nullable: false),
                    MaxTotalExposure = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ScopeType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalAuthorities", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "DeviationCatalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviationCatalog", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalAuthorities_Tier_Priority",
                table: "ApprovalAuthorities",
                columns: new[] { "Tier", "Priority" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalAuthorities");

            migrationBuilder.DropTable(
                name: "DeviationCatalog");
        }
    }
}
