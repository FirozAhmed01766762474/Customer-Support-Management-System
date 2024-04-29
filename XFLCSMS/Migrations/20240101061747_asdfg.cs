using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XFLCSMS.Migrations
{
    public partial class asdfg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AffectedSectionss",
                columns: table => new
                {
                    AffectedSectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ASection = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffectedSectionss", x => x.AffectedSectionId);
                });

            migrationBuilder.CreateTable(
                name: "Brokerages",
                columns: table => new
                {
                    BrokerageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrokerageHouseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrokerageHouseAcronym = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brokerages", x => x.BrokerageId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Cust_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Cust_Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportCatagories",
                columns: table => new
                {
                    SupportCatagoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SCatagory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportCatagories", x => x.SupportCatagoryId);
                });

            migrationBuilder.CreateTable(
                name: "SupportSubCatagories",
                columns: table => new
                {
                    SupportSubCatagoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubCatagory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportSubCatagories", x => x.SupportSubCatagoryId);
                });

            migrationBuilder.CreateTable(
                name: "SupportTypes",
                columns: table => new
                {
                    SupportTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTypes", x => x.SupportTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhonNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrokerageHouseName = table.Column<int>(type: "int", nullable: false),
                    BrokerageHouseAcronym = table.Column<int>(type: "int", nullable: false),
                    Branch = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    VerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UCatagory = table.Column<bool>(type: "bit", nullable: false),
                    UType = table.Column<bool>(type: "bit", nullable: false),
                    UStatus = table.Column<bool>(type: "bit", nullable: false),
                    Terms = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branchhs",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrokerageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branchhs", x => x.BranchId);
                    table.ForeignKey(
                        name: "FK_Branchhs_Brokerages_BrokerageId",
                        column: x => x.BrokerageId,
                        principalTable: "Brokerages",
                        principalColumn: "BrokerageId");
                });

            migrationBuilder.CreateTable(
                name: "Issues",
                columns: table => new
                {
                    IssueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BrokerageId = table.Column<int>(type: "int", nullable: false),
                    TDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportTypeId = table.Column<int>(type: "int", nullable: true),
                    SupportCatagoryId = table.Column<int>(type: "int", nullable: true),
                    SupportSubCatagoryId = table.Column<int>(type: "int", nullable: true),
                    AffectedSectionId = table.Column<int>(type: "int", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ITitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchhBranchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issues", x => x.IssueId);
                    table.ForeignKey(
                        name: "FK_Issues_AffectedSectionss_AffectedSectionId",
                        column: x => x.AffectedSectionId,
                        principalTable: "AffectedSectionss",
                        principalColumn: "AffectedSectionId");
                    table.ForeignKey(
                        name: "FK_Issues_Branchhs_BranchhBranchId",
                        column: x => x.BranchhBranchId,
                        principalTable: "Branchhs",
                        principalColumn: "BranchId");
                    table.ForeignKey(
                        name: "FK_Issues_Brokerages_BrokerageId",
                        column: x => x.BrokerageId,
                        principalTable: "Brokerages",
                        principalColumn: "BrokerageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Issues_SupportCatagories_SupportCatagoryId",
                        column: x => x.SupportCatagoryId,
                        principalTable: "SupportCatagories",
                        principalColumn: "SupportCatagoryId");
                    table.ForeignKey(
                        name: "FK_Issues_SupportSubCatagories_SupportSubCatagoryId",
                        column: x => x.SupportSubCatagoryId,
                        principalTable: "SupportSubCatagories",
                        principalColumn: "SupportSubCatagoryId");
                    table.ForeignKey(
                        name: "FK_Issues_SupportTypes_SupportTypeId",
                        column: x => x.SupportTypeId,
                        principalTable: "SupportTypes",
                        principalColumn: "SupportTypeId");
                    table.ForeignKey(
                        name: "FK_Issues_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttachmentLoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.AttachmentId);
                    table.ForeignKey(
                        name: "FK_Attachments_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "IssueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_IssueId",
                table: "Attachments",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Branchhs_BrokerageId",
                table: "Branchhs",
                column: "BrokerageId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_AffectedSectionId",
                table: "Issues",
                column: "AffectedSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_BranchhBranchId",
                table: "Issues",
                column: "BranchhBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_BrokerageId",
                table: "Issues",
                column: "BrokerageId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_SupportCatagoryId",
                table: "Issues",
                column: "SupportCatagoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_SupportSubCatagoryId",
                table: "Issues",
                column: "SupportSubCatagoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_SupportTypeId",
                table: "Issues",
                column: "SupportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_UserId",
                table: "Issues",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Issues");

            migrationBuilder.DropTable(
                name: "AffectedSectionss");

            migrationBuilder.DropTable(
                name: "Branchhs");

            migrationBuilder.DropTable(
                name: "SupportCatagories");

            migrationBuilder.DropTable(
                name: "SupportSubCatagories");

            migrationBuilder.DropTable(
                name: "SupportTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Brokerages");
        }
    }
}
