using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppCore.Migrations
{
    /// <inheritdoc />
    public partial class AnnouncementsGroupContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EventDate",
                table: "App_Posts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SocialGroupId",
                table: "App_Posts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "App_PostComments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "App_Announcements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TickerText = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPriority = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SocialGroupId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PublishRequested = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PublishRequestedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsPublishedToWeb = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PublishedToWebAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PublishedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReviewedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReviewedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ReviewNotes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShowOnHomePage = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Announcements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Announcements_App_SocialGroups_SocialGroupId",
                        column: x => x.SocialGroupId,
                        principalTable: "App_SocialGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_App_Posts_SocialGroupId",
                table: "App_Posts",
                column: "SocialGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Announcements_SocialGroupId",
                table: "App_Announcements",
                column: "SocialGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_App_Posts_App_SocialGroups_SocialGroupId",
                table: "App_Posts",
                column: "SocialGroupId",
                principalTable: "App_SocialGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_App_Posts_App_SocialGroups_SocialGroupId",
                table: "App_Posts");

            migrationBuilder.DropTable(
                name: "App_Announcements");

            migrationBuilder.DropIndex(
                name: "IX_App_Posts_SocialGroupId",
                table: "App_Posts");

            migrationBuilder.DropColumn(
                name: "EventDate",
                table: "App_Posts");

            migrationBuilder.DropColumn(
                name: "SocialGroupId",
                table: "App_Posts");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "App_PostComments");
        }
    }
}
