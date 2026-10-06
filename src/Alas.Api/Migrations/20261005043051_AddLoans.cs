using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alas.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLoans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LamId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApplicationGroupNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreationTypeCode = table.Column<int>(type: "int", nullable: true),
                    CreationTypeLabel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CisId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Suffix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Agency = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NetTakeHomePay = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LengthOfService = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LoanNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Product = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProposedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TermDays = table.Column<int>(type: "int", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    PolicyTermMonths = table.Column<int>(type: "int", nullable: true),
                    NotarialFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DocStamps = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Insurance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossProceeds = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetProceedsToClient = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalExposure = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MonthlyAmortization = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    HasDeviations = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AoRecommendation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApplicationDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastActionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LoanType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    AssignedApproverId = table.Column<int>(type: "int", nullable: true),
                    AssignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DocumentsCompleteAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinTermDays = table.Column<int>(type: "int", nullable: false),
                    MaxTermDays = table.Column<int>(type: "int", nullable: false),
                    NotarialFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DocStampFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InsuranceFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AdvanceInterestRate = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    ApplicationChargeRate = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    AmortizationMode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ChargeAdvanceInterest = table.Column<bool>(type: "bit", nullable: false),
                    IsRetired = table.Column<bool>(type: "bit", nullable: false),
                    LastSyncedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FromStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ToStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ActionByUserId = table.Column<int>(type: "int", nullable: false),
                    ActionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActionByRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanActions_LoanApplications_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanActions_LoanApplicationId",
                table: "LoanActions",
                column: "LoanApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_BranchCode_Status",
                table: "LoanApplications",
                columns: new[] { "BranchCode", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_CisId_BranchCode",
                table: "LoanApplications",
                columns: new[] { "CisId", "BranchCode" });

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_CreatedById_Status",
                table: "LoanApplications",
                columns: new[] { "CreatedById", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_LamId",
                table: "LoanApplications",
                column: "LamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_LoanNo",
                table: "LoanApplications",
                column: "LoanNo");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_Status",
                table: "LoanApplications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_LoanProducts_Code",
                table: "LoanProducts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanProducts_IsRetired",
                table: "LoanProducts",
                column: "IsRetired");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanActions");

            migrationBuilder.DropTable(
                name: "LoanProducts");

            migrationBuilder.DropTable(
                name: "LoanApplications");
        }
    }
}
