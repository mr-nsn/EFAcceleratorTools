using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFAcceleratorTools.Examples.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_INSTRUCTOR",
                columns: table => new
                {
                    SQ_INSTRUCTOR = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TX_FULL_NAME = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    DT_CREATION = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SQ_INSTRUCTOR", x => x.SQ_INSTRUCTOR);
                });

            migrationBuilder.CreateTable(
                name: "TB_COURSE",
                columns: table => new
                {
                    SQ_COURSE = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SQ_INSTRUCTOR = table.Column<long>(type: "bigint", nullable: false),
                    TX_TITLE = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    DT_CREATION = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SQ_COURSE", x => x.SQ_COURSE);
                    table.ForeignKey(
                        name: "FK_TB_COURSE_TB_INSTRUCTOR_SQ_INSTRUCTOR",
                        column: x => x.SQ_INSTRUCTOR,
                        principalTable: "TB_INSTRUCTOR",
                        principalColumn: "SQ_INSTRUCTOR",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_PROFILE",
                columns: table => new
                {
                    SQ_PROFILE = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SQ_INSTRUCTOR = table.Column<long>(type: "bigint", nullable: false),
                    TX_BIO = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    TX_LINKEDIN = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    DT_CREATION = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SQ_PROFILE", x => x.SQ_PROFILE);
                    table.ForeignKey(
                        name: "FK_TB_PROFILE_TB_INSTRUCTOR_SQ_INSTRUCTOR",
                        column: x => x.SQ_INSTRUCTOR,
                        principalTable: "TB_INSTRUCTOR",
                        principalColumn: "SQ_INSTRUCTOR",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_MODULE",
                columns: table => new
                {
                    SQ_MODULE = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SQ_COURSE = table.Column<long>(type: "bigint", nullable: false),
                    TX_NAME = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    DT_CREATION = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SQ_MODULE", x => x.SQ_MODULE);
                    table.ForeignKey(
                        name: "FK_TB_MODULE_TB_COURSE_SQ_COURSE",
                        column: x => x.SQ_COURSE,
                        principalTable: "TB_COURSE",
                        principalColumn: "SQ_COURSE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_LESSON",
                columns: table => new
                {
                    SQ_LESSON = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SQ_MODULE = table.Column<long>(type: "bigint", nullable: false),
                    TX_TITLE = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    DT_DURATION = table.Column<TimeSpan>(type: "time", nullable: false),
                    DT_CREATION = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SQ_LESSON", x => x.SQ_LESSON);
                    table.ForeignKey(
                        name: "FK_TB_LESSON_TB_MODULE_SQ_MODULE",
                        column: x => x.SQ_MODULE,
                        principalTable: "TB_MODULE",
                        principalColumn: "SQ_MODULE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_COURSE_SQ_INSTRUCTOR",
                table: "TB_COURSE",
                column: "SQ_INSTRUCTOR");

            migrationBuilder.CreateIndex(
                name: "IX_TB_LESSON_SQ_MODULE",
                table: "TB_LESSON",
                column: "SQ_MODULE");

            migrationBuilder.CreateIndex(
                name: "IX_TB_MODULE_SQ_COURSE",
                table: "TB_MODULE",
                column: "SQ_COURSE");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PROFILE_SQ_INSTRUCTOR",
                table: "TB_PROFILE",
                column: "SQ_INSTRUCTOR",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_LESSON");

            migrationBuilder.DropTable(
                name: "TB_PROFILE");

            migrationBuilder.DropTable(
                name: "TB_MODULE");

            migrationBuilder.DropTable(
                name: "TB_COURSE");

            migrationBuilder.DropTable(
                name: "TB_INSTRUCTOR");
        }
    }
}
