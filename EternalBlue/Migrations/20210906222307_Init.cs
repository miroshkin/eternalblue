using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EternalBlue.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessedCandidates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedCandidates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TechnologyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    TechnologyName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillId);
                });

            migrationBuilder.CreateTable(
                name: "ProcessedCandidateSkills",
                columns: table => new
                {
                    ProcessedCandidateSkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessedCandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedCandidateSkills", x => x.ProcessedCandidateSkillId);
                    table.ForeignKey(
                        name: "FK_ProcessedCandidateSkills_ProcessedCandidates_ProcessedCandidateId",
                        column: x => x.ProcessedCandidateId,
                        principalTable: "ProcessedCandidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessedCandidateSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedCandidateSkills_ProcessedCandidateId",
                table: "ProcessedCandidateSkills",
                column: "ProcessedCandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedCandidateSkills_SkillId",
                table: "ProcessedCandidateSkills",
                column: "SkillId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessedCandidateSkills");

            migrationBuilder.DropTable(
                name: "ProcessedCandidates");

            migrationBuilder.DropTable(
                name: "Skills");
        }
    }
}
