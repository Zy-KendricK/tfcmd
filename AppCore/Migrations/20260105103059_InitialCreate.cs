using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppCore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_AboutPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Subtitle = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FeaturedImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Section = table.Column<int>(type: "int", nullable: false),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_AboutPages", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_AdvertCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_AdvertCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_AdvertCategories_App_AdvertCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "App_AdvertCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IconUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Color = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ItemCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Categories_App_Categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "App_Categories",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Charities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tagline = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LogoUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CoverImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Website = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsFeatured = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Charities", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ContactCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailRecipient = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ContactCategories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Subject = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsGroupConversation = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LastMessageAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    MessageCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Conversations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_FaqCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_FaqCategories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Forums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IconUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentForumId = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Visibility = table.Column<int>(type: "int", nullable: false),
                    TopicPermission = table.Column<int>(type: "int", nullable: false),
                    ReplyPermission = table.Column<int>(type: "int", nullable: false),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TopicCount = table.Column<int>(type: "int", nullable: false),
                    PostCount = table.Column<int>(type: "int", nullable: false),
                    LastPostId = table.Column<int>(type: "int", nullable: true),
                    LastActivityAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Forums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Forums_App_Forums_ParentForumId",
                        column: x => x.ParentForumId,
                        principalTable: "App_Forums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_JobCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_JobCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_JobCategories_App_JobCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "App_JobCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Menus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Location = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Menus", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Pages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MetaTitle = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MetaDescription = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MetaKeywords = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FeaturedImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentPageId = table.Column<int>(type: "int", nullable: true),
                    Template = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ShowInNavigation = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Pages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Pages_App_Pages_ParentPageId",
                        column: x => x.ParentPageId,
                        principalTable: "App_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Module = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Permissions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsFeatured = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ProductCategories_App_ProductCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "App_ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Group = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValueType = table.Column<int>(type: "int", nullable: false),
                    IsPublic = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEditable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Settings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Tags", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_TeamMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Department = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Bio = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhotoUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LinkedInUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TwitterUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsLeadership = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_TeamMembers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_UserGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Color = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsSystemGroup = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_UserGroups", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_VideoCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_VideoCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_VideoCategories_App_VideoCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "App_VideoCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Bio = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AvatarUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CoverImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Location = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Website = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsSuperAdmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_SocialGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AvatarUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CoverImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Rules = table.Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MemberCount = table.Column<int>(type: "int", nullable: false),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_SocialGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_SocialGroups_App_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "App_Categories",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_CharityProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharityId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GoalAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    CurrentAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    Currency = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DonorCount = table.Column<int>(type: "int", nullable: false),
                    IsFeatured = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_CharityProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_CharityProjects_App_Charities_CharityId",
                        column: x => x.CharityId,
                        principalTable: "App_Charities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Faqs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Question = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Answer = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    HelpfulCount = table.Column<int>(type: "int", nullable: false),
                    NotHelpfulCount = table.Column<int>(type: "int", nullable: false),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Faqs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Faqs_App_FaqCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "App_FaqCategories",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MenuId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PageId = table.Column<int>(type: "int", nullable: true),
                    ParentItemId = table.Column<int>(type: "int", nullable: true),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CssClass = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Target = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_MenuItems_App_MenuItems_ParentItemId",
                        column: x => x.ParentItemId,
                        principalTable: "App_MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_App_MenuItems_App_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "App_Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_MenuItems_App_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "App_Pages",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_GroupPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserGroupId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    IsGranted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AssignedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_GroupPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_GroupPermissions_App_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "App_Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_GroupPermissions_App_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "App_UserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActivityType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RelatedEntityType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RelatedEntityId = table.Column<int>(type: "int", nullable: true),
                    Privacy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Activities_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Adverts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PriceType = table.Column<int>(type: "int", nullable: false),
                    IsNegotiable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Condition = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Latitude = table.Column<double>(type: "double", nullable: true),
                    Longitude = table.Column<double>(type: "double", nullable: true),
                    PostedById = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContactEmail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContactPhone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShowContactInfo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsFeatured = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsUrgent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    InquiryCount = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Adverts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Adverts_App_AdvertCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "App_AdvertCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_Adverts_AspNetUsers_PostedById",
                        column: x => x.PostedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Action = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    OldValues = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NewValues = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddress = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserAgent = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_AuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Subject = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedToId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddress = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ContactMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ContactMessages_App_ContactCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "App_ContactCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_ContactMessages_AspNetUsers_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_ContactMessages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ConversationParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ConversationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UnreadCount = table.Column<int>(type: "int", nullable: false),
                    LastReadAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsArchived = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsMuted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ConversationParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ConversationParticipants_App_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "App_Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_ConversationParticipants_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ForumModerators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ForumId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ForumModerators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ForumModerators_App_Forums_ForumId",
                        column: x => x.ForumId,
                        principalTable: "App_Forums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_ForumModerators_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyLogoUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Location = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RemoteOption = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    SalaryMin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalaryMax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalaryCurrency = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SalaryPeriod = table.Column<int>(type: "int", nullable: true),
                    ShowSalary = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ApplicationDeadline = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ApplicationUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApplicationEmail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExperienceLevel = table.Column<int>(type: "int", nullable: true),
                    RequiredSkills = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Benefits = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PostedById = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsFeatured = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsUrgent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    ApplicationCount = table.Column<int>(type: "int", nullable: false),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Jobs_App_JobCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "App_JobCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_Jobs_AspNetUsers_PostedById",
                        column: x => x.PostedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_MediaFolders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentFolderId = table.Column<int>(type: "int", nullable: true),
                    OwnerId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_MediaFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_MediaFolders_App_MediaFolders_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "App_MediaFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_App_MediaFolders_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SenderId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RecipientId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConversationId = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SenderDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RecipientDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Messages_App_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "App_Conversations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_Messages_AspNetUsers_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_Messages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Link = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SenderId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Notifications_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_PhotoAlbums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OwnerId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CoverPhotoId = table.Column<int>(type: "int", nullable: true),
                    CoverUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Privacy = table.Column<int>(type: "int", nullable: false),
                    PhotoCount = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    AllowContributions = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_PhotoAlbums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_PhotoAlbums_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Playlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OwnerId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThumbnailUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Privacy = table.Column<int>(type: "int", nullable: false),
                    VideoCount = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Playlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Playlists_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Excerpt = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FeaturedImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Format = table.Column<int>(type: "int", nullable: false),
                    VideoUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AudioUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LinkUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    CommentCount = table.Column<int>(type: "int", nullable: false),
                    AllowComments = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsFeatured = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MetaTitle = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MetaDescription = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Posts_App_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "App_Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_Posts_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SKU = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShortDescription = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CompareAtPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: true),
                    TrackInventory = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AllowBackorder = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Weight = table.Column<double>(type: "double", nullable: true),
                    Length = table.Column<double>(type: "double", nullable: true),
                    Width = table.Column<double>(type: "double", nullable: true),
                    Height = table.Column<double>(type: "double", nullable: true),
                    SellerId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsFeatured = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsOnSale = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsBestseller = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsNewArrival = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    SoldCount = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    DownloadUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DownloadLimit = table.Column<int>(type: "int", nullable: true),
                    DownloadExpiry = table.Column<int>(type: "int", nullable: true),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Products_App_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "App_ProductCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_Products_AspNetUsers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_UserGroupMemberships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserGroupId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    JoinedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AddedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPrimaryGroup = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_UserGroupMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_UserGroupMemberships_App_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "App_UserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_UserGroupMemberships_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Videos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThumbnailUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PreviewUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<int>(type: "int", nullable: false),
                    ExternalId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmbedUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    UploadedById = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    MimeType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Resolution = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Privacy = table.Column<int>(type: "int", nullable: false),
                    IsFeatured = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    DislikeCount = table.Column<int>(type: "int", nullable: false),
                    CommentCount = table.Column<int>(type: "int", nullable: false),
                    ShareCount = table.Column<int>(type: "int", nullable: false),
                    AllowComments = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AllowDownload = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AllowEmbed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsAgeRestricted = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Videos_App_VideoCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "App_VideoCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_Videos_AspNetUsers_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_SocialGroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SocialGroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    InvitedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsApproved = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsBanned = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BannedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    BanReason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_SocialGroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_SocialGroupMembers_App_SocialGroups_SocialGroupId",
                        column: x => x.SocialGroupId,
                        principalTable: "App_SocialGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_SocialGroupMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_SocialGroupPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SocialGroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MediaUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPinned = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    CommentCount = table.Column<int>(type: "int", nullable: false),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_SocialGroupPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_SocialGroupPosts_App_SocialGroups_SocialGroupId",
                        column: x => x.SocialGroupId,
                        principalTable: "App_SocialGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_SocialGroupPosts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ActivityComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentCommentId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ActivityComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ActivityComments_App_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "App_Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_ActivityComments_App_ActivityComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "App_ActivityComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_App_ActivityComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ActivityLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ActivityLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ActivityLikes_App_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "App_Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_ActivityLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_AdvertFavorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AdvertId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_AdvertFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_AdvertFavorites_App_Adverts_AdvertId",
                        column: x => x.AdvertId,
                        principalTable: "App_Adverts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_AdvertFavorites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_AdvertImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AdvertId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThumbnailUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AltText = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_AdvertImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_AdvertImages_App_Adverts_AdvertId",
                        column: x => x.AdvertId,
                        principalTable: "App_Adverts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_AdvertInquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AdvertId = table.Column<int>(type: "int", nullable: false),
                    InquirerId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GuestName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GuestEmail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GuestPhone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsReplied = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Reply = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RepliedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_AdvertInquiries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_AdvertInquiries_App_Adverts_AdvertId",
                        column: x => x.AdvertId,
                        principalTable: "App_Adverts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_AdvertInquiries_AspNetUsers_InquirerId",
                        column: x => x.InquirerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ContactMessageReplies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    ContactMessageId = table.Column<int>(type: "int", nullable: false),
                    ReplierId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailSent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EmailSentAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ContactMessageReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ContactMessageReplies_App_ContactMessages_ContactMessage~",
                        column: x => x.ContactMessageId,
                        principalTable: "App_ContactMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_ContactMessageReplies_AspNetUsers_ReplierId",
                        column: x => x.ReplierId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_JobApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    ApplicantId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CoverLetter = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResumeUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_JobApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_JobApplications_App_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "App_Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_JobApplications_AspNetUsers_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_MediaFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThumbnailUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MimeType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UploadedById = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FolderId = table.Column<int>(type: "int", nullable: true),
                    AltText = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_MediaFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_MediaFiles_App_MediaFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "App_MediaFolders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_MediaFiles_AspNetUsers_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_MessageAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MimeType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_MessageAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_MessageAttachments_App_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "App_Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_Photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThumbnailUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MediumUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OriginalUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AlbumId = table.Column<int>(type: "int", nullable: true),
                    UploadedById = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    MimeType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Camera = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Location = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Latitude = table.Column<double>(type: "double", nullable: true),
                    Longitude = table.Column<double>(type: "double", nullable: true),
                    TakenAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Privacy = table.Column<int>(type: "int", nullable: false),
                    IsFeatured = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    DownloadCount = table.Column<int>(type: "int", nullable: false),
                    AllowDownload = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AllowComments = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Photos_App_PhotoAlbums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "App_PhotoAlbums",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_Photos_AspNetUsers_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_PostComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentCommentId = table.Column<int>(type: "int", nullable: true),
                    IsApproved = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_PostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_PostComments_App_PostComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "App_PostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_App_PostComments_App_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "App_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_PostComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_PostImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Caption = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_PostImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_PostImages_App_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "App_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_PostTags",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_PostTags", x => new { x.PostId, x.TagId });
                    table.ForeignKey(
                        name: "FK_App_PostTags_App_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "App_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_PostTags_App_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "App_Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThumbnailUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AltText = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ProductImages_App_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "App_Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ProductReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Comment = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsVerifiedPurchase = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsApproved = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HelpfulCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ProductReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ProductReviews_App_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "App_Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_ProductReviews_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ProductTags",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ProductTags", x => new { x.ProductId, x.TagId });
                    table.ForeignKey(
                        name: "FK_App_ProductTags_App_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "App_Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_ProductTags_App_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "App_Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ProductVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SKU = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CompareAtPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: true),
                    ImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Options = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ProductVariants_App_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "App_Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_VideoComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VideoId = table.Column<int>(type: "int", nullable: false),
                    CommenterId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentCommentId = table.Column<int>(type: "int", nullable: true),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    DislikeCount = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<int>(type: "int", nullable: true),
                    IsPinned = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_VideoComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_VideoComments_App_VideoComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "App_VideoComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_App_VideoComments_App_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "App_Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_VideoComments_AspNetUsers_CommenterId",
                        column: x => x.CommenterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_VideoLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VideoId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLike = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_VideoLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_VideoLikes_App_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "App_Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_VideoLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_VideoPlaylists",
                columns: table => new
                {
                    VideoId = table.Column<int>(type: "int", nullable: false),
                    PlaylistId = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_VideoPlaylists", x => new { x.VideoId, x.PlaylistId });
                    table.ForeignKey(
                        name: "FK_App_VideoPlaylists_App_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "App_Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_VideoPlaylists_App_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "App_Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_PhotoComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PhotoId = table.Column<int>(type: "int", nullable: false),
                    CommenterId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentCommentId = table.Column<int>(type: "int", nullable: true),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_PhotoComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_PhotoComments_App_PhotoComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "App_PhotoComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_App_PhotoComments_App_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "App_Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_PhotoComments_AspNetUsers_CommenterId",
                        column: x => x.CommenterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_PhotoLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PhotoId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_PhotoLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_PhotoLikes_App_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "App_Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_PhotoLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_PhotoTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PhotoId = table.Column<int>(type: "int", nullable: false),
                    TaggedUserId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TagText = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PositionX = table.Column<double>(type: "double", nullable: true),
                    PositionY = table.Column<double>(type: "double", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_PhotoTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_PhotoTags_App_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "App_Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_PhotoTags_AspNetUsers_TaggedUserId",
                        column: x => x.TaggedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ForumPollOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PollId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VoteCount = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ForumPollOptions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ForumPollVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OptionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ForumPollVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ForumPollVotes_App_ForumPollOptions_OptionId",
                        column: x => x.OptionId,
                        principalTable: "App_ForumPollOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_ForumPollVotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ForumPolls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AllowMultiple = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ShowResults = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TotalVotes = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ForumPolls", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ForumPostLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ForumPostLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ForumPostLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ForumPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentPostId = table.Column<int>(type: "int", nullable: true),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    IsEdited = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EditReason = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsBestAnswer = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ForumPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ForumPosts_App_ForumPosts_ParentPostId",
                        column: x => x.ParentPostId,
                        principalTable: "App_ForumPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_App_ForumPosts_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ForumTopics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ForumId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsSticky = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsLocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsAnnouncement = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    ReplyCount = table.Column<int>(type: "int", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    LastPostId = table.Column<int>(type: "int", nullable: true),
                    LastActivityAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
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
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ForumTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ForumTopics_App_ForumPosts_LastPostId",
                        column: x => x.LastPostId,
                        principalTable: "App_ForumPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_App_ForumTopics_App_Forums_ForumId",
                        column: x => x.ForumId,
                        principalTable: "App_Forums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_ForumTopics_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ForumTopicSubscribers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailNotification = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedById = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ForumTopicSubscribers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ForumTopicSubscribers_App_ForumTopics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "App_ForumTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_ForumTopicSubscribers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "App_ForumTopicTags",
                columns: table => new
                {
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ForumTopicTags", x => new { x.TopicId, x.TagId });
                    table.ForeignKey(
                        name: "FK_App_ForumTopicTags_App_ForumTopics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "App_ForumTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_ForumTopicTags_App_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "App_Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "App_Permissions",
                columns: new[] { "Id", "Category", "Code", "CreatedAt", "CreatedById", "DeletedAt", "Description", "DisplayOrder", "IsActive", "IsDeleted", "Module", "Name", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 1, "Dashboard", "dashboard.view", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(620), null, null, null, 0, true, false, "Dashboard", "View Dashboard", null, null },
                    { 2, "Users", "users.view", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(630), null, null, null, 0, true, false, "Users", "View Users", null, null },
                    { 3, "Users", "users.create", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(630), null, null, null, 0, true, false, "Users", "Create Users", null, null },
                    { 4, "Users", "users.edit", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(630), null, null, null, 0, true, false, "Users", "Edit Users", null, null },
                    { 5, "Users", "users.delete", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(640), null, null, null, 0, true, false, "Users", "Delete Users", null, null },
                    { 6, "Groups", "groups.view", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(640), null, null, null, 0, true, false, "Groups", "View Groups", null, null },
                    { 7, "Groups", "groups.manage", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(640), null, null, null, 0, true, false, "Groups", "Manage Groups", null, null },
                    { 8, "Content", "posts.view", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(640), null, null, null, 0, true, false, "Posts", "View Posts", null, null },
                    { 9, "Content", "posts.create", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(640), null, null, null, 0, true, false, "Posts", "Create Posts", null, null },
                    { 10, "Content", "posts.edit", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(640), null, null, null, 0, true, false, "Posts", "Edit Posts", null, null },
                    { 11, "Content", "posts.delete", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(650), null, null, null, 0, true, false, "Posts", "Delete Posts", null, null },
                    { 12, "Content", "posts.publish", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(650), null, null, null, 0, true, false, "Posts", "Publish Posts", null, null },
                    { 13, "Content", "jobs.view", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(650), null, null, null, 0, true, false, "Jobs", "View Jobs", null, null },
                    { 14, "Content", "jobs.create", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(650), null, null, null, 0, true, false, "Jobs", "Create Jobs", null, null },
                    { 15, "Content", "jobs.edit", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(650), null, null, null, 0, true, false, "Jobs", "Edit Jobs", null, null },
                    { 16, "Content", "jobs.delete", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(650), null, null, null, 0, true, false, "Jobs", "Delete Jobs", null, null },
                    { 17, "Content", "jobs.publish", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(660), null, null, null, 0, true, false, "Jobs", "Publish Jobs", null, null },
                    { 18, "Content", "adverts.view", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(660), null, null, null, 0, true, false, "Adverts", "View Adverts", null, null },
                    { 19, "Content", "adverts.create", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(660), null, null, null, 0, true, false, "Adverts", "Create Adverts", null, null },
                    { 20, "Content", "adverts.edit", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(660), null, null, null, 0, true, false, "Adverts", "Edit Adverts", null, null },
                    { 21, "Content", "adverts.delete", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(660), null, null, null, 0, true, false, "Adverts", "Delete Adverts", null, null },
                    { 22, "Content", "adverts.publish", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(660), null, null, null, 0, true, false, "Adverts", "Publish Adverts", null, null },
                    { 23, "Shop", "products.view", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(670), null, null, null, 0, true, false, "Products", "View Products", null, null },
                    { 24, "Shop", "products.create", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(670), null, null, null, 0, true, false, "Products", "Create Products", null, null },
                    { 25, "Shop", "products.edit", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(670), null, null, null, 0, true, false, "Products", "Edit Products", null, null },
                    { 26, "Shop", "products.delete", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(670), null, null, null, 0, true, false, "Products", "Delete Products", null, null },
                    { 27, "Shop", "products.publish", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(670), null, null, null, 0, true, false, "Products", "Publish Products", null, null },
                    { 28, "Media", "photos.view", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(670), null, null, null, 0, true, false, "Photos", "View Photos", null, null },
                    { 29, "Media", "photos.upload", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(670), null, null, null, 0, true, false, "Photos", "Upload Photos", null, null },
                    { 30, "Media", "photos.delete", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(670), null, null, null, 0, true, false, "Photos", "Delete Photos", null, null },
                    { 31, "Media", "videos.view", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(670), null, null, null, 0, true, false, "Videos", "View Videos", null, null },
                    { 32, "Media", "videos.upload", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(680), null, null, null, 0, true, false, "Videos", "Upload Videos", null, null },
                    { 33, "Media", "videos.delete", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(680), null, null, null, 0, true, false, "Videos", "Delete Videos", null, null },
                    { 34, "Community", "forums.view", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(680), null, null, null, 0, true, false, "Forums", "View Forums", null, null },
                    { 35, "Community", "forums.manage", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(680), null, null, null, 0, true, false, "Forums", "Manage Forums", null, null },
                    { 36, "Community", "forums.moderate", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(680), null, null, null, 0, true, false, "Forums", "Moderate Forums", null, null },
                    { 37, "System", "settings.view", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(680), null, null, null, 0, true, false, "Settings", "View Settings", null, null },
                    { 38, "System", "settings.edit", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(680), null, null, null, 0, true, false, "Settings", "Edit Settings", null, null },
                    { 39, "System", "audit.view", new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(690), null, null, null, 0, true, false, "Audit", "View Audit Logs", null, null }
                });

            migrationBuilder.InsertData(
                table: "App_Settings",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "Description", "Group", "IsActive", "IsDeleted", "IsEditable", "IsPublic", "Key", "UpdatedAt", "UpdatedById", "Value", "ValueType" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(4310), null, null, "Site name", "General", true, false, true, false, "site.name", null, null, "Social Platform", 0 },
                    { 2, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(4320), null, null, "Site description", "General", true, false, true, false, "site.description", null, null, "A modern social networking platform", 0 },
                    { 3, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(4320), null, null, "Site logo URL", "General", true, false, true, false, "site.logo", null, null, "/images/logo.png", 0 },
                    { 4, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(4420), null, null, "Allow user registration", "Registration", true, false, true, false, "registration.enabled", null, null, "true", 2 },
                    { 5, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(4420), null, null, "Require admin approval for new users", "Registration", true, false, true, false, "registration.requireApproval", null, null, "false", 2 },
                    { 6, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(4420), null, null, "Require review before publishing", "Content", true, false, true, false, "content.requireReview", null, null, "true", 2 }
                });

            migrationBuilder.InsertData(
                table: "App_UserGroups",
                columns: new[] { "Id", "Color", "CreatedAt", "CreatedById", "DeletedAt", "Description", "DisplayOrder", "Icon", "IsActive", "IsDeleted", "IsSystemGroup", "Name", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 1, 5, 10, 30, 58, 761, DateTimeKind.Utc).AddTicks(6990), null, null, "Full system access", 0, null, true, false, true, "Administrators", null, null },
                    { 2, null, new DateTime(2026, 1, 5, 10, 30, 58, 761, DateTimeKind.Utc).AddTicks(7140), null, null, "Can review and publish content", 0, null, true, false, true, "Reviewers", null, null },
                    { 3, null, new DateTime(2026, 1, 5, 10, 30, 58, 761, DateTimeKind.Utc).AddTicks(7140), null, null, "Can create and edit content", 0, null, true, false, true, "Editors", null, null },
                    { 4, null, new DateTime(2026, 1, 5, 10, 30, 58, 761, DateTimeKind.Utc).AddTicks(7140), null, null, "Regular platform members", 0, null, true, false, true, "Members", null, null }
                });

            migrationBuilder.InsertData(
                table: "App_GroupPermissions",
                columns: new[] { "Id", "AssignedAt", "AssignedById", "CreatedAt", "IsActive", "IsDeleted", "IsGranted", "PermissionId", "UserGroupId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(2700), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3310), true, false, true, 1, 1 },
                    { 2, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3490), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3490), true, false, true, 2, 1 },
                    { 3, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3520), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3520), true, false, true, 3, 1 },
                    { 4, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3520), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3520), true, false, true, 4, 1 },
                    { 5, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3520), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), true, false, true, 5, 1 },
                    { 6, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), true, false, true, 6, 1 },
                    { 7, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), true, false, true, 7, 1 },
                    { 8, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), true, false, true, 8, 1 },
                    { 9, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), true, false, true, 9, 1 },
                    { 10, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), true, false, true, 10, 1 },
                    { 11, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), true, false, true, 11, 1 },
                    { 12, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), true, false, true, 12, 1 },
                    { 13, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), true, false, true, 13, 1 },
                    { 14, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), true, false, true, 14, 1 },
                    { 15, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), true, false, true, 15, 1 },
                    { 16, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3530), true, false, true, 16, 1 },
                    { 17, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), true, false, true, 17, 1 },
                    { 18, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), true, false, true, 18, 1 },
                    { 19, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), true, false, true, 19, 1 },
                    { 20, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), true, false, true, 20, 1 },
                    { 21, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), true, false, true, 21, 1 },
                    { 22, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), true, false, true, 22, 1 },
                    { 23, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), true, false, true, 23, 1 },
                    { 24, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), true, false, true, 24, 1 },
                    { 25, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), true, false, true, 25, 1 },
                    { 26, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), true, false, true, 26, 1 },
                    { 27, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3540), true, false, true, 27, 1 },
                    { 28, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), true, false, true, 28, 1 },
                    { 29, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), true, false, true, 29, 1 },
                    { 30, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), true, false, true, 30, 1 },
                    { 31, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), true, false, true, 31, 1 },
                    { 32, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), true, false, true, 32, 1 },
                    { 33, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), true, false, true, 33, 1 },
                    { 34, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), true, false, true, 34, 1 },
                    { 35, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), true, false, true, 35, 1 },
                    { 36, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), true, false, true, 36, 1 },
                    { 37, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), true, false, true, 37, 1 },
                    { 38, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), true, false, true, 38, 1 },
                    { 39, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), null, new DateTime(2026, 1, 5, 10, 30, 58, 762, DateTimeKind.Utc).AddTicks(3550), true, false, true, 39, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_App_Activities_UserId",
                table: "App_Activities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ActivityComments_ActivityId",
                table: "App_ActivityComments",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ActivityComments_ParentCommentId",
                table: "App_ActivityComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ActivityComments_UserId",
                table: "App_ActivityComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ActivityLikes_ActivityId_UserId",
                table: "App_ActivityLikes",
                columns: new[] { "ActivityId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_ActivityLikes_UserId",
                table: "App_ActivityLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_AdvertCategories_ParentCategoryId",
                table: "App_AdvertCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_App_AdvertCategories_Slug",
                table: "App_AdvertCategories",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_App_AdvertFavorites_AdvertId_UserId",
                table: "App_AdvertFavorites",
                columns: new[] { "AdvertId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_AdvertFavorites_UserId",
                table: "App_AdvertFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_AdvertImages_AdvertId",
                table: "App_AdvertImages",
                column: "AdvertId");

            migrationBuilder.CreateIndex(
                name: "IX_App_AdvertInquiries_AdvertId",
                table: "App_AdvertInquiries",
                column: "AdvertId");

            migrationBuilder.CreateIndex(
                name: "IX_App_AdvertInquiries_InquirerId",
                table: "App_AdvertInquiries",
                column: "InquirerId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Adverts_CategoryId",
                table: "App_Adverts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Adverts_PostedById",
                table: "App_Adverts",
                column: "PostedById");

            migrationBuilder.CreateIndex(
                name: "IX_App_Adverts_Slug",
                table: "App_Adverts",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_App_Adverts_Status",
                table: "App_Adverts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_App_AuditLogs_UserId",
                table: "App_AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Categories_ParentId",
                table: "App_Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_App_CharityProjects_CharityId",
                table: "App_CharityProjects",
                column: "CharityId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ContactMessageReplies_ContactMessageId",
                table: "App_ContactMessageReplies",
                column: "ContactMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ContactMessageReplies_ReplierId",
                table: "App_ContactMessageReplies",
                column: "ReplierId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ContactMessages_AssignedToId",
                table: "App_ContactMessages",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ContactMessages_CategoryId",
                table: "App_ContactMessages",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ContactMessages_UserId",
                table: "App_ContactMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ConversationParticipants_ConversationId",
                table: "App_ConversationParticipants",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ConversationParticipants_UserId",
                table: "App_ConversationParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Faqs_CategoryId",
                table: "App_Faqs",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumModerators_ForumId",
                table: "App_ForumModerators",
                column: "ForumId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumModerators_UserId",
                table: "App_ForumModerators",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumPollOptions_PollId",
                table: "App_ForumPollOptions",
                column: "PollId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumPolls_TopicId",
                table: "App_ForumPolls",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumPollVotes_OptionId",
                table: "App_ForumPollVotes",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumPollVotes_UserId",
                table: "App_ForumPollVotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumPostLikes_PostId_UserId",
                table: "App_ForumPostLikes",
                columns: new[] { "PostId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumPostLikes_UserId",
                table: "App_ForumPostLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumPosts_AuthorId",
                table: "App_ForumPosts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumPosts_ParentPostId",
                table: "App_ForumPosts",
                column: "ParentPostId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumPosts_TopicId",
                table: "App_ForumPosts",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Forums_ParentForumId",
                table: "App_Forums",
                column: "ParentForumId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumTopics_AuthorId",
                table: "App_ForumTopics",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumTopics_ForumId",
                table: "App_ForumTopics",
                column: "ForumId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumTopics_LastPostId",
                table: "App_ForumTopics",
                column: "LastPostId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumTopicSubscribers_TopicId",
                table: "App_ForumTopicSubscribers",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumTopicSubscribers_UserId",
                table: "App_ForumTopicSubscribers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ForumTopicTags_TagId",
                table: "App_ForumTopicTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_App_GroupPermissions_PermissionId",
                table: "App_GroupPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_App_GroupPermissions_UserGroupId",
                table: "App_GroupPermissions",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_App_JobApplications_ApplicantId",
                table: "App_JobApplications",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_App_JobApplications_JobId",
                table: "App_JobApplications",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_App_JobCategories_ParentCategoryId",
                table: "App_JobCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_App_JobCategories_Slug",
                table: "App_JobCategories",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_App_Jobs_CategoryId",
                table: "App_Jobs",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Jobs_PostedById",
                table: "App_Jobs",
                column: "PostedById");

            migrationBuilder.CreateIndex(
                name: "IX_App_Jobs_Slug",
                table: "App_Jobs",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_App_Jobs_Status",
                table: "App_Jobs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_App_MediaFiles_FolderId",
                table: "App_MediaFiles",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_App_MediaFiles_UploadedById",
                table: "App_MediaFiles",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_App_MediaFolders_OwnerId",
                table: "App_MediaFolders",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_App_MediaFolders_ParentFolderId",
                table: "App_MediaFolders",
                column: "ParentFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_App_MenuItems_MenuId",
                table: "App_MenuItems",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_App_MenuItems_PageId",
                table: "App_MenuItems",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_App_MenuItems_ParentItemId",
                table: "App_MenuItems",
                column: "ParentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_App_MessageAttachments_MessageId",
                table: "App_MessageAttachments",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Messages_ConversationId",
                table: "App_Messages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Messages_RecipientId",
                table: "App_Messages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Messages_SenderId",
                table: "App_Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Notifications_SenderId",
                table: "App_Notifications",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Notifications_UserId",
                table: "App_Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Pages_ParentPageId",
                table: "App_Pages",
                column: "ParentPageId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Pages_Slug",
                table: "App_Pages",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_Permissions_Code",
                table: "App_Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_PhotoAlbums_OwnerId",
                table: "App_PhotoAlbums",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_App_PhotoComments_CommenterId",
                table: "App_PhotoComments",
                column: "CommenterId");

            migrationBuilder.CreateIndex(
                name: "IX_App_PhotoComments_ParentCommentId",
                table: "App_PhotoComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_App_PhotoComments_PhotoId",
                table: "App_PhotoComments",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_App_PhotoLikes_PhotoId_UserId",
                table: "App_PhotoLikes",
                columns: new[] { "PhotoId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_PhotoLikes_UserId",
                table: "App_PhotoLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Photos_AlbumId",
                table: "App_Photos",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Photos_UploadedById",
                table: "App_Photos",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_App_PhotoTags_PhotoId",
                table: "App_PhotoTags",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_App_PhotoTags_TaggedUserId",
                table: "App_PhotoTags",
                column: "TaggedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Playlists_OwnerId",
                table: "App_Playlists",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_App_PostComments_ParentCommentId",
                table: "App_PostComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_App_PostComments_PostId",
                table: "App_PostComments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_App_PostComments_UserId",
                table: "App_PostComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_PostImages_PostId",
                table: "App_PostImages",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Posts_AuthorId",
                table: "App_Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Posts_CategoryId",
                table: "App_Posts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Posts_Slug",
                table: "App_Posts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_Posts_Status",
                table: "App_Posts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_App_PostTags_TagId",
                table: "App_PostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ProductCategories_ParentCategoryId",
                table: "App_ProductCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ProductCategories_Slug",
                table: "App_ProductCategories",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_App_ProductImages_ProductId",
                table: "App_ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ProductReviews_ProductId",
                table: "App_ProductReviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ProductReviews_ReviewerId",
                table: "App_ProductReviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Products_CategoryId",
                table: "App_Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Products_SellerId",
                table: "App_Products",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Products_Slug",
                table: "App_Products",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_App_Products_Status",
                table: "App_Products",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_App_ProductTags_TagId",
                table: "App_ProductTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ProductVariants_ProductId",
                table: "App_ProductVariants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Settings_Key",
                table: "App_Settings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_SocialGroupMembers_SocialGroupId",
                table: "App_SocialGroupMembers",
                column: "SocialGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_App_SocialGroupMembers_UserId",
                table: "App_SocialGroupMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_SocialGroupPosts_SocialGroupId",
                table: "App_SocialGroupPosts",
                column: "SocialGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_App_SocialGroupPosts_UserId",
                table: "App_SocialGroupPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_SocialGroups_CategoryId",
                table: "App_SocialGroups",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Tags_Slug",
                table: "App_Tags",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_UserGroupMemberships_UserGroupId",
                table: "App_UserGroupMemberships",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_App_UserGroupMemberships_UserId",
                table: "App_UserGroupMemberships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_UserGroups_Name",
                table: "App_UserGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_VideoCategories_ParentCategoryId",
                table: "App_VideoCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_App_VideoCategories_Slug",
                table: "App_VideoCategories",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_App_VideoComments_CommenterId",
                table: "App_VideoComments",
                column: "CommenterId");

            migrationBuilder.CreateIndex(
                name: "IX_App_VideoComments_ParentCommentId",
                table: "App_VideoComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_App_VideoComments_VideoId",
                table: "App_VideoComments",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_App_VideoLikes_UserId",
                table: "App_VideoLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_App_VideoLikes_VideoId_UserId",
                table: "App_VideoLikes",
                columns: new[] { "VideoId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_VideoPlaylists_PlaylistId",
                table: "App_VideoPlaylists",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Videos_CategoryId",
                table: "App_Videos",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Videos_UploadedById",
                table: "App_Videos",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IsSuperAdmin",
                table: "AspNetUsers",
                column: "IsSuperAdmin");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_App_ForumPollOptions_App_ForumPolls_PollId",
                table: "App_ForumPollOptions",
                column: "PollId",
                principalTable: "App_ForumPolls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_App_ForumPolls_App_ForumTopics_TopicId",
                table: "App_ForumPolls",
                column: "TopicId",
                principalTable: "App_ForumTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_App_ForumPostLikes_App_ForumPosts_PostId",
                table: "App_ForumPostLikes",
                column: "PostId",
                principalTable: "App_ForumPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_App_ForumPosts_App_ForumTopics_TopicId",
                table: "App_ForumPosts",
                column: "TopicId",
                principalTable: "App_ForumTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_App_ForumPosts_AspNetUsers_AuthorId",
                table: "App_ForumPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_App_ForumTopics_AspNetUsers_AuthorId",
                table: "App_ForumTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_App_ForumTopics_App_Forums_ForumId",
                table: "App_ForumTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_App_ForumPosts_App_ForumTopics_TopicId",
                table: "App_ForumPosts");

            migrationBuilder.DropTable(
                name: "App_AboutPages");

            migrationBuilder.DropTable(
                name: "App_ActivityComments");

            migrationBuilder.DropTable(
                name: "App_ActivityLikes");

            migrationBuilder.DropTable(
                name: "App_AdvertFavorites");

            migrationBuilder.DropTable(
                name: "App_AdvertImages");

            migrationBuilder.DropTable(
                name: "App_AdvertInquiries");

            migrationBuilder.DropTable(
                name: "App_AuditLogs");

            migrationBuilder.DropTable(
                name: "App_CharityProjects");

            migrationBuilder.DropTable(
                name: "App_ContactMessageReplies");

            migrationBuilder.DropTable(
                name: "App_ConversationParticipants");

            migrationBuilder.DropTable(
                name: "App_Faqs");

            migrationBuilder.DropTable(
                name: "App_ForumModerators");

            migrationBuilder.DropTable(
                name: "App_ForumPollVotes");

            migrationBuilder.DropTable(
                name: "App_ForumPostLikes");

            migrationBuilder.DropTable(
                name: "App_ForumTopicSubscribers");

            migrationBuilder.DropTable(
                name: "App_ForumTopicTags");

            migrationBuilder.DropTable(
                name: "App_GroupPermissions");

            migrationBuilder.DropTable(
                name: "App_JobApplications");

            migrationBuilder.DropTable(
                name: "App_MediaFiles");

            migrationBuilder.DropTable(
                name: "App_MenuItems");

            migrationBuilder.DropTable(
                name: "App_MessageAttachments");

            migrationBuilder.DropTable(
                name: "App_Notifications");

            migrationBuilder.DropTable(
                name: "App_PhotoComments");

            migrationBuilder.DropTable(
                name: "App_PhotoLikes");

            migrationBuilder.DropTable(
                name: "App_PhotoTags");

            migrationBuilder.DropTable(
                name: "App_PostComments");

            migrationBuilder.DropTable(
                name: "App_PostImages");

            migrationBuilder.DropTable(
                name: "App_PostTags");

            migrationBuilder.DropTable(
                name: "App_ProductImages");

            migrationBuilder.DropTable(
                name: "App_ProductReviews");

            migrationBuilder.DropTable(
                name: "App_ProductTags");

            migrationBuilder.DropTable(
                name: "App_ProductVariants");

            migrationBuilder.DropTable(
                name: "App_Settings");

            migrationBuilder.DropTable(
                name: "App_SocialGroupMembers");

            migrationBuilder.DropTable(
                name: "App_SocialGroupPosts");

            migrationBuilder.DropTable(
                name: "App_TeamMembers");

            migrationBuilder.DropTable(
                name: "App_UserGroupMemberships");

            migrationBuilder.DropTable(
                name: "App_VideoComments");

            migrationBuilder.DropTable(
                name: "App_VideoLikes");

            migrationBuilder.DropTable(
                name: "App_VideoPlaylists");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "App_Activities");

            migrationBuilder.DropTable(
                name: "App_Adverts");

            migrationBuilder.DropTable(
                name: "App_Charities");

            migrationBuilder.DropTable(
                name: "App_ContactMessages");

            migrationBuilder.DropTable(
                name: "App_FaqCategories");

            migrationBuilder.DropTable(
                name: "App_ForumPollOptions");

            migrationBuilder.DropTable(
                name: "App_Permissions");

            migrationBuilder.DropTable(
                name: "App_Jobs");

            migrationBuilder.DropTable(
                name: "App_MediaFolders");

            migrationBuilder.DropTable(
                name: "App_Menus");

            migrationBuilder.DropTable(
                name: "App_Pages");

            migrationBuilder.DropTable(
                name: "App_Messages");

            migrationBuilder.DropTable(
                name: "App_Photos");

            migrationBuilder.DropTable(
                name: "App_Posts");

            migrationBuilder.DropTable(
                name: "App_Tags");

            migrationBuilder.DropTable(
                name: "App_Products");

            migrationBuilder.DropTable(
                name: "App_SocialGroups");

            migrationBuilder.DropTable(
                name: "App_UserGroups");

            migrationBuilder.DropTable(
                name: "App_Playlists");

            migrationBuilder.DropTable(
                name: "App_Videos");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "App_AdvertCategories");

            migrationBuilder.DropTable(
                name: "App_ContactCategories");

            migrationBuilder.DropTable(
                name: "App_ForumPolls");

            migrationBuilder.DropTable(
                name: "App_JobCategories");

            migrationBuilder.DropTable(
                name: "App_Conversations");

            migrationBuilder.DropTable(
                name: "App_PhotoAlbums");

            migrationBuilder.DropTable(
                name: "App_ProductCategories");

            migrationBuilder.DropTable(
                name: "App_Categories");

            migrationBuilder.DropTable(
                name: "App_VideoCategories");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "App_Forums");

            migrationBuilder.DropTable(
                name: "App_ForumTopics");

            migrationBuilder.DropTable(
                name: "App_ForumPosts");
        }
    }
}
