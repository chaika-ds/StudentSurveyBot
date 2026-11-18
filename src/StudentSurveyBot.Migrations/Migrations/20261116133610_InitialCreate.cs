using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StudentSurveyBot.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    TelegramUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    InterestCodes = table.Column<string[]>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConsentAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TgId = table.Column<long>(type: "bigint", nullable: false),
                    At = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsentAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Interests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Label = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TgId = table.Column<long>(type: "bigint", nullable: false),
                    TgUsername = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Direction = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Course = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdditionalInfos",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecialWishes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ContactTelegramNick = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalInfos", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_AdditionalInfos_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunityJoinIntents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CommunityId = table.Column<int>(type: "integer", nullable: false),
                    ClickedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityJoinIntents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityJoinIntents_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityJoinIntents_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consents",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    GivenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    TextHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consents", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_Consents_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentInterests",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    InterestId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentInterests", x => new { x.StudentId, x.InterestId });
                    table.ForeignKey(
                        name: "FK_StudentInterests_Interests_InterestId",
                        column: x => x.InterestId,
                        principalTable: "Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentInterests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentPreferences",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesiredVibe = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ContactStyle = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    GroupSize = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    VisitFrequency = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PreferredTime = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Budget = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentPreferences", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_StudentPreferences_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveySessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CurrentStep = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveySessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveySessions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommunityJoinIntents_CommunityId",
                table: "CommunityJoinIntents",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityJoinIntents_StudentId",
                table: "CommunityJoinIntents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Interests_Code",
                table: "Interests",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentInterests_InterestId",
                table: "StudentInterests",
                column: "InterestId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_TgId",
                table: "Students",
                column: "TgId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveySessions_StudentId",
                table: "SurveySessions",
                column: "StudentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdditionalInfos");

            migrationBuilder.DropTable(
                name: "CommunityJoinIntents");

            migrationBuilder.DropTable(
                name: "ConsentAudits");

            migrationBuilder.DropTable(
                name: "Consents");

            migrationBuilder.DropTable(
                name: "StudentInterests");

            migrationBuilder.DropTable(
                name: "StudentPreferences");

            migrationBuilder.DropTable(
                name: "SurveySessions");

            migrationBuilder.DropTable(
                name: "Communities");

            migrationBuilder.DropTable(
                name: "Interests");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
