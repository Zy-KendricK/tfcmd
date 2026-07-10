using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppCore.Migrations
{
    /// <inheritdoc />
    public partial class IntendedForWebAndGalleryPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_Videos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_TeamMembers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_SocialGroups",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_SocialGroupPosts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_Products",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_Posts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_Playlists",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_Photos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_PhotoAlbums",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_Pages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_Jobs",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_ForumTopics",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_Faqs",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_CharityProjects",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_CharityPageItems",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_Charities",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_Announcements",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_Adverts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntendedForWeb",
                table: "App_AboutPages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "App_Permissions",
                columns: new[] { "Id", "Category", "Code", "CreatedAt", "CreatedById", "DeletedAt", "Description", "DisplayOrder", "IsActive", "IsDeleted", "Module", "Name", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 42, "Media", "photos.publish", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, 0, true, false, "Photos", "Publish Photos", null, null },
                    { 43, "Media", "videos.publish", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, 0, true, false, "Videos", "Publish Videos", null, null }
                });

            migrationBuilder.InsertData(
                table: "App_GroupPermissions",
                columns: new[] { "Id", "AssignedAt", "AssignedById", "CreatedAt", "IsActive", "IsDeleted", "IsGranted", "PermissionId", "UserGroupId" },
                values: new object[,]
                {
                    { 42, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, false, true, 42, 1 },
                    { 43, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, false, true, 43, 1 }
                });

            // Backfill: anything already published to the web (or awaiting review) was clearly
            // intended for the website, so flag it to keep the existing workflow intact.
            foreach (var table in new[]
            {
                "App_Videos", "App_TeamMembers", "App_SocialGroups", "App_SocialGroupPosts",
                "App_Products", "App_Posts", "App_Playlists", "App_Photos", "App_PhotoAlbums",
                "App_Pages", "App_Jobs", "App_ForumTopics", "App_Faqs", "App_CharityProjects",
                "App_CharityPageItems", "App_Charities", "App_Announcements", "App_Adverts",
                "App_AboutPages"
            })
            {
                migrationBuilder.Sql(
                    $"UPDATE `{table}` SET `IntendedForWeb` = 1 WHERE `IsPublishedToWeb` = 1 OR `PublishRequested` = 1 OR `Status` IN (1, 2, 3);");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "App_GroupPermissions",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "App_GroupPermissions",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "App_Permissions",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "App_Permissions",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_Videos");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_TeamMembers");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_SocialGroups");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_SocialGroupPosts");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_Products");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_Posts");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_Playlists");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_Photos");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_PhotoAlbums");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_Pages");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_Jobs");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_ForumTopics");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_Faqs");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_CharityProjects");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_CharityPageItems");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_Charities");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_Announcements");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_Adverts");

            migrationBuilder.DropColumn(
                name: "IntendedForWeb",
                table: "App_AboutPages");
        }
    }
}
