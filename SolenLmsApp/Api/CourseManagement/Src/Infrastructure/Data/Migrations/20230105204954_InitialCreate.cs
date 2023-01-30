using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CourseManagement");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "CourseManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instructors",
                schema: "CourseManagement",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FamilyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Learners",
                schema: "CourseManagement",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FamilyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Learners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                schema: "CourseManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InstructorId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalSchema: "CourseManagement",
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CourseCategories",
                schema: "CourseManagement",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCategories", x => new { x.CourseId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_CourseCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "CourseManagement",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseCategories_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "CourseManagement",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearnerProgress",
                schema: "CourseManagement",
                columns: table => new
                {
                    LearnerId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Progress = table.Column<float>(type: "real", nullable: false),
                    LastAccessTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnerProgress", x => new { x.LearnerId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_LearnerProgress_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "CourseManagement",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearnerProgress_Learners_LearnerId",
                        column: x => x.LearnerId,
                        principalSchema: "CourseManagement",
                        principalTable: "Learners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Module",
                schema: "CourseManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Module_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "CourseManagement",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lecture",
                schema: "CourseManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    ResourceId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lecture_Module_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "CourseManagement",
                        principalTable: "Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_OrganizationId",
                schema: "CourseManagement",
                table: "Categories",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCategories_CategoryId",
                schema: "CourseManagement",
                table: "CourseCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstructorId",
                schema: "CourseManagement",
                table: "Courses",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_OrganizationId",
                schema: "CourseManagement",
                table: "Courses",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_OrganizationId",
                schema: "CourseManagement",
                table: "Instructors",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_LearnerProgress_CourseId",
                schema: "CourseManagement",
                table: "LearnerProgress",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Learners_OrganizationId",
                schema: "CourseManagement",
                table: "Learners",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_ModuleId",
                schema: "CourseManagement",
                table: "Lecture",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_ResourceId",
                schema: "CourseManagement",
                table: "Lecture",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Module_CourseId",
                schema: "CourseManagement",
                table: "Module",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseCategories",
                schema: "CourseManagement");

            migrationBuilder.DropTable(
                name: "LearnerProgress",
                schema: "CourseManagement");

            migrationBuilder.DropTable(
                name: "Lecture",
                schema: "CourseManagement");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "CourseManagement");

            migrationBuilder.DropTable(
                name: "Learners",
                schema: "CourseManagement");

            migrationBuilder.DropTable(
                name: "Module",
                schema: "CourseManagement");

            migrationBuilder.DropTable(
                name: "Courses",
                schema: "CourseManagement");

            migrationBuilder.DropTable(
                name: "Instructors",
                schema: "CourseManagement");
        }
    }
}
