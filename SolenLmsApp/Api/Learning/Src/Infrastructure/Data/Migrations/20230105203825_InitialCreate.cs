using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imanys.SolenLms.Application.Learning.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Learning");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "Learning",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instructors",
                schema: "Learning",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FamilyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Learners",
                schema: "Learning",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Learners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                schema: "Learning",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    InstructorId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Learning",
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Courses_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalSchema: "Learning",
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BookmarkedCourse",
                schema: "Learning",
                columns: table => new
                {
                    LearnerId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CourseId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookmarkedCourse", x => new { x.LearnerId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_BookmarkedCourse_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "Learning",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseCategories",
                schema: "Learning",
                columns: table => new
                {
                    CourseId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCategories", x => new { x.CourseId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_CourseCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Learning",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseCategories_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "Learning",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearnerCourseProgress",
                schema: "Learning",
                columns: table => new
                {
                    LearnerId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CourseId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Progress = table.Column<float>(type: "real", nullable: false),
                    LastAccessTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnerCourseProgress", x => new { x.LearnerId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_LearnerCourseProgress_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "Learning",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearnerCourseProgress_Learners_LearnerId",
                        column: x => x.LearnerId,
                        principalSchema: "Learning",
                        principalTable: "Learners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Module",
                schema: "Learning",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CourseId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Module_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "Learning",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lecture",
                schema: "Learning",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ModuleId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
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
                        principalSchema: "Learning",
                        principalTable: "Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearnerCourseAccess",
                schema: "Learning",
                columns: table => new
                {
                    LearnerId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LectureId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CourseId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccessTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnerCourseAccess", x => new { x.LearnerId, x.LectureId });
                    table.ForeignKey(
                        name: "FK_LearnerCourseAccess_Learners_LearnerId",
                        column: x => x.LearnerId,
                        principalSchema: "Learning",
                        principalTable: "Learners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearnerCourseAccess_Lecture_LectureId",
                        column: x => x.LectureId,
                        principalSchema: "Learning",
                        principalTable: "Lecture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookmarkedCourse_CourseId",
                schema: "Learning",
                table: "BookmarkedCourse",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_OrganizationId",
                schema: "Learning",
                table: "Categories",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCategories_CategoryId",
                schema: "Learning",
                table: "CourseCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CategoryId",
                schema: "Learning",
                table: "Courses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstructorId",
                schema: "Learning",
                table: "Courses",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_IsPublished",
                schema: "Learning",
                table: "Courses",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_OrganizationId",
                schema: "Learning",
                table: "Courses",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_OrganizationId",
                schema: "Learning",
                table: "Instructors",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_LearnerCourseAccess_LectureId",
                schema: "Learning",
                table: "LearnerCourseAccess",
                column: "LectureId");

            migrationBuilder.CreateIndex(
                name: "IX_LearnerCourseProgress_CourseId",
                schema: "Learning",
                table: "LearnerCourseProgress",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Learners_OrganizationId",
                schema: "Learning",
                table: "Learners",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_ModuleId",
                schema: "Learning",
                table: "Lecture",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Module_CourseId",
                schema: "Learning",
                table: "Module",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookmarkedCourse",
                schema: "Learning");

            migrationBuilder.DropTable(
                name: "CourseCategories",
                schema: "Learning");

            migrationBuilder.DropTable(
                name: "LearnerCourseAccess",
                schema: "Learning");

            migrationBuilder.DropTable(
                name: "LearnerCourseProgress",
                schema: "Learning");

            migrationBuilder.DropTable(
                name: "Lecture",
                schema: "Learning");

            migrationBuilder.DropTable(
                name: "Learners",
                schema: "Learning");

            migrationBuilder.DropTable(
                name: "Module",
                schema: "Learning");

            migrationBuilder.DropTable(
                name: "Courses",
                schema: "Learning");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "Learning");

            migrationBuilder.DropTable(
                name: "Instructors",
                schema: "Learning");
        }
    }
}
