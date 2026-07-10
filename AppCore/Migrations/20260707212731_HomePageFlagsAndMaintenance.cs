using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppCore.Migrations
{
    /// <inheritdoc />
    public partial class HomePageFlagsAndMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_Videos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_TeamMembers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_SocialGroups",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_SocialGroupPosts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_Products",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HomeSection",
                table: "App_Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_Posts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_Playlists",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_Photos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_PhotoAlbums",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_Pages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_Jobs",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_ForumTopics",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_Faqs",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_CharityProjects",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_Charities",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_Categories",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_Adverts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "App_AboutPages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "App_Permissions",
                columns: new[] { "Id", "Category", "Code", "CreatedAt", "CreatedById", "DeletedAt", "Description", "DisplayOrder", "IsActive", "IsDeleted", "Module", "Name", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 40, "System", "maintenance.cache", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, 0, true, false, "Maintenance", "Clear Website Cache & Data", null, null },
                    { 41, "Content", "content.homepage", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, 0, true, false, "Posts", "Manage Home Page Content", null, null }
                });

            migrationBuilder.InsertData(
                table: "App_Settings",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "Description", "Group", "IsActive", "IsDeleted", "IsEditable", "IsPublic", "Key", "UpdatedAt", "UpdatedById", "Value", "ValueType" },
                values: new object[] { 7, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Website cache version stamp; bumping it forces the website to reload data from the database", "System", true, false, true, false, "site.cacheVersion", null, null, "1", 1 });

            migrationBuilder.InsertData(
                table: "App_GroupPermissions",
                columns: new[] { "Id", "AssignedAt", "AssignedById", "CreatedAt", "IsActive", "IsDeleted", "IsGranted", "PermissionId", "UserGroupId" },
                values: new object[,]
                {
                    { 40, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, false, true, 40, 1 },
                    { 41, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, false, true, 41, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "App_GroupPermissions",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "App_GroupPermissions",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "App_Settings",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "App_Permissions",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "App_Permissions",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_Videos");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_TeamMembers");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_SocialGroups");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_SocialGroupPosts");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_Products");

            migrationBuilder.DropColumn(
                name: "HomeSection",
                table: "App_Posts");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_Posts");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_Playlists");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_Photos");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_PhotoAlbums");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_Pages");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_Jobs");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_ForumTopics");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_Faqs");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_CharityProjects");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_Charities");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_Categories");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_Adverts");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "App_AboutPages");
        }
    }
}
