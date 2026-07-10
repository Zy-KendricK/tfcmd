using AppCore.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    // Static seed timestamp so the model stays deterministic between builds
    // (dynamic DateTime.UtcNow in HasData causes perpetual pending-model-change warnings).
    private static readonly DateTime SeedDate = new(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc);

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Categories
    public DbSet<Category> Categories { get; set; }

    // User & Groups
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<GroupPermission> GroupPermissions { get; set; }
    public DbSet<UserGroupMembership> UserGroupMemberships { get; set; }

    // Activity & Social
    public DbSet<Activity> Activities { get; set; }
    public DbSet<ActivityComment> ActivityComments { get; set; }
    public DbSet<ActivityLike> ActivityLikes { get; set; }
    public DbSet<SocialGroup> SocialGroups { get; set; }
    public DbSet<SocialGroupMember> SocialGroupMembers { get; set; }
    public DbSet<SocialGroupPost> SocialGroupPosts { get; set; }

    // Posts & Blog
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostComment> PostComments { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<PostImage> PostImages { get; set; }

    // Jobs
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobCategory> JobCategories { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }

    // Adverts
    public DbSet<Advert> Adverts { get; set; }
    public DbSet<AdvertCategory> AdvertCategories { get; set; }
    public DbSet<AdvertImage> AdvertImages { get; set; }
    public DbSet<AdvertInquiry> AdvertInquiries { get; set; }
    public DbSet<AdvertFavorite> AdvertFavorites { get; set; }

    // Products & Shop
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }
    public DbSet<ProductTag> ProductTags { get; set; }

    // Photos
    public DbSet<Photo> Photos { get; set; }
    public DbSet<PhotoAlbum> PhotoAlbums { get; set; }
    public DbSet<PhotoComment> PhotoComments { get; set; }
    public DbSet<PhotoLike> PhotoLikes { get; set; }
    public DbSet<PhotoTag> PhotoTags { get; set; }

    // Videos
    public DbSet<Video> Videos { get; set; }
    public DbSet<VideoCategory> VideoCategories { get; set; }
    public DbSet<VideoComment> VideoComments { get; set; }
    public DbSet<VideoLike> VideoLikes { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<VideoPlaylist> VideoPlaylists { get; set; }

    // Forums
    public DbSet<Forum> Forums { get; set; }
    public DbSet<ForumTopic> ForumTopics { get; set; }
    public DbSet<ForumPost> ForumPosts { get; set; }
    public DbSet<ForumPostLike> ForumPostLikes { get; set; }
    public DbSet<ForumModerator> ForumModerators { get; set; }
    public DbSet<ForumTopicTag> ForumTopicTags { get; set; }
    public DbSet<ForumTopicSubscriber> ForumTopicSubscribers { get; set; }
    public DbSet<ForumPoll> ForumPolls { get; set; }
    public DbSet<ForumPollOption> ForumPollOptions { get; set; }
    public DbSet<ForumPollVote> ForumPollVotes { get; set; }

    // Content & Pages
    public DbSet<Faq> Faqs { get; set; }
    public DbSet<FaqCategory> FaqCategories { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }
    public DbSet<ContactCategory> ContactCategories { get; set; }
    public DbSet<ContactMessageReply> ContactMessageReplies { get; set; }
    public DbSet<AboutPage> AboutPages { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<Charity> Charities { get; set; }
    public DbSet<CharityProject> CharityProjects { get; set; }
    public DbSet<CharityPageItem> CharityPageItems { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }

    // System
    public DbSet<Setting> Settings { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
    public DbSet<MessageAttachment> MessageAttachments { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<MediaFolder> MediaFolders { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure table names with prefix
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName != null && !tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName("App_" + tableName);
            }
        }

        // User relationships
        ConfigureUserRelationships(builder);

        // Configure composite keys
        ConfigureCompositeKeys(builder);

        // Configure indexes
        ConfigureIndexes(builder);

        // Configure delete behaviors
        ConfigureDeleteBehaviors(builder);

        // Seed initial data
        SeedData(builder);
    }

    private static void ConfigureUserRelationships(ModelBuilder builder)
    {
        // ApplicationUser relationships
        builder.Entity<ApplicationUser>()
            .HasMany(u => u.GroupMemberships)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Activities)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Posts)
            .WithOne(p => p.Author)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureCompositeKeys(ModelBuilder builder)
    {
        // PostTag composite key
        builder.Entity<PostTag>()
            .HasKey(pt => new { pt.PostId, pt.TagId });

        // ProductTag composite key
        builder.Entity<ProductTag>()
            .HasKey(pt => new { pt.ProductId, pt.TagId });

        // ForumTopicTag composite key
        builder.Entity<ForumTopicTag>()
            .HasKey(ft => new { ft.TopicId, ft.TagId });

        // VideoPlaylist composite key
        builder.Entity<VideoPlaylist>()
            .HasKey(vp => new { vp.VideoId, vp.PlaylistId });

        // Unique constraints
        builder.Entity<AdvertFavorite>()
            .HasIndex(af => new { af.AdvertId, af.UserId })
            .IsUnique();

        builder.Entity<ActivityLike>()
            .HasIndex(al => new { al.ActivityId, al.UserId })
            .IsUnique();

        builder.Entity<PhotoLike>()
            .HasIndex(pl => new { pl.PhotoId, pl.UserId })
            .IsUnique();

        builder.Entity<VideoLike>()
            .HasIndex(vl => new { vl.VideoId, vl.UserId })
            .IsUnique();

        builder.Entity<ForumPostLike>()
            .HasIndex(fl => new { fl.PostId, fl.UserId })
            .IsUnique();
    }

    private static void ConfigureIndexes(ModelBuilder builder)
    {
        // User indexes
        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email);

        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.IsSuperAdmin);

        // Slug indexes
        builder.Entity<Post>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        builder.Entity<Job>()
            .HasIndex(j => j.Slug);

        builder.Entity<Advert>()
            .HasIndex(a => a.Slug);

        builder.Entity<Product>()
            .HasIndex(p => p.Slug);

        builder.Entity<Page>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        // Status indexes for publishing workflow
        builder.Entity<Post>()
            .HasIndex(p => p.Status);

        builder.Entity<Job>()
            .HasIndex(j => j.Status);

        builder.Entity<Advert>()
            .HasIndex(a => a.Status);

        builder.Entity<Product>()
            .HasIndex(p => p.Status);

        // Category indexes
        builder.Entity<JobCategory>()
            .HasIndex(c => c.Slug);

        builder.Entity<AdvertCategory>()
            .HasIndex(c => c.Slug);

        builder.Entity<ProductCategory>()
            .HasIndex(c => c.Slug);

        builder.Entity<VideoCategory>()
            .HasIndex(c => c.Slug);

        // Permission indexes
        builder.Entity<Permission>()
            .HasIndex(p => p.Code)
            .IsUnique();

        builder.Entity<UserGroup>()
            .HasIndex(g => g.Name)
            .IsUnique();

        // Settings index
        builder.Entity<Setting>()
            .HasIndex(s => s.Key)
            .IsUnique();

        // Tag index
        builder.Entity<Tag>()
            .HasIndex(t => t.Slug)
            .IsUnique();
    }

    private static void ConfigureDeleteBehaviors(ModelBuilder builder)
    {
        // Prevent cascade delete for self-referencing entities
        builder.Entity<JobCategory>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.ChildCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AdvertCategory>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.ChildCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ProductCategory>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.ChildCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<VideoCategory>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.ChildCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Forum>()
            .HasOne(f => f.ParentForum)
            .WithMany(f => f.ChildForums)
            .HasForeignKey(f => f.ParentForumId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PostComment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Post>()
            .HasOne(p => p.SocialGroup)
            .WithMany()
            .HasForeignKey(p => p.SocialGroupId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Announcement>()
            .HasOne(a => a.SocialGroup)
            .WithMany()
            .HasForeignKey(a => a.SocialGroupId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ActivityComment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<VideoComment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PhotoComment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ForumPost>()
            .HasOne(p => p.ParentPost)
            .WithMany(p => p.Replies)
            .HasForeignKey(p => p.ParentPostId)
            .OnDelete(DeleteBehavior.Restrict);

        // Forum relationships
        builder.Entity<ForumPost>()
            .HasOne(p => p.Topic)
            .WithMany(t => t.Posts)
            .HasForeignKey(p => p.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ForumTopic>()
            .HasOne(t => t.LastPost)
            .WithMany()
            .HasForeignKey(t => t.LastPostId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ForumTopic>()
            .HasOne(t => t.Forum)
            .WithMany(f => f.Topics)
            .HasForeignKey(t => t.ForumId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Page>()
            .HasOne(p => p.ParentPage)
            .WithMany(p => p.ChildPages)
            .HasForeignKey(p => p.ParentPageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MenuItem>()
            .HasOne(m => m.ParentItem)
            .WithMany(m => m.ChildItems)
            .HasForeignKey(m => m.ParentItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MediaFolder>()
            .HasOne(f => f.ParentFolder)
            .WithMany(f => f.ChildFolders)
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void SeedData(ModelBuilder builder)
    {
        // Seed default user groups
        builder.Entity<UserGroup>().HasData(
            new UserGroup { Id = 1, Name = "Administrators", Description = "Full system access", IsSystemGroup = true, IsActive = true, CreatedAt = SeedDate },
            new UserGroup { Id = 2, Name = "Reviewers", Description = "Can review and publish content", IsSystemGroup = true, IsActive = true, CreatedAt = SeedDate },
            new UserGroup { Id = 3, Name = "Editors", Description = "Can create and edit content", IsSystemGroup = true, IsActive = true, CreatedAt = SeedDate },
            new UserGroup { Id = 4, Name = "Members", Description = "Regular platform members", IsSystemGroup = true, IsActive = true, CreatedAt = SeedDate }
        );

        // Seed permissions
        var permissions = new List<Permission>
        {
            // Dashboard
            new() { Id = 1, Code = "dashboard.view", Name = "View Dashboard", Category = "Dashboard", Module = "Dashboard", IsActive = true, CreatedAt = SeedDate },
            
            // Users
            new() { Id = 2, Code = "users.view", Name = "View Users", Category = "Users", Module = "Users", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 3, Code = "users.create", Name = "Create Users", Category = "Users", Module = "Users", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 4, Code = "users.edit", Name = "Edit Users", Category = "Users", Module = "Users", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 5, Code = "users.delete", Name = "Delete Users", Category = "Users", Module = "Users", IsActive = true, CreatedAt = SeedDate },
            
            // Groups
            new() { Id = 6, Code = "groups.view", Name = "View Groups", Category = "Groups", Module = "Groups", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 7, Code = "groups.manage", Name = "Manage Groups", Category = "Groups", Module = "Groups", IsActive = true, CreatedAt = SeedDate },
            
            // Posts
            new() { Id = 8, Code = "posts.view", Name = "View Posts", Category = "Content", Module = "Posts", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 9, Code = "posts.create", Name = "Create Posts", Category = "Content", Module = "Posts", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 10, Code = "posts.edit", Name = "Edit Posts", Category = "Content", Module = "Posts", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 11, Code = "posts.delete", Name = "Delete Posts", Category = "Content", Module = "Posts", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 12, Code = "posts.publish", Name = "Publish Posts", Category = "Content", Module = "Posts", IsActive = true, CreatedAt = SeedDate },
            
            // Jobs
            new() { Id = 13, Code = "jobs.view", Name = "View Jobs", Category = "Content", Module = "Jobs", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 14, Code = "jobs.create", Name = "Create Jobs", Category = "Content", Module = "Jobs", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 15, Code = "jobs.edit", Name = "Edit Jobs", Category = "Content", Module = "Jobs", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 16, Code = "jobs.delete", Name = "Delete Jobs", Category = "Content", Module = "Jobs", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 17, Code = "jobs.publish", Name = "Publish Jobs", Category = "Content", Module = "Jobs", IsActive = true, CreatedAt = SeedDate },
            
            // Adverts
            new() { Id = 18, Code = "adverts.view", Name = "View Adverts", Category = "Content", Module = "Adverts", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 19, Code = "adverts.create", Name = "Create Adverts", Category = "Content", Module = "Adverts", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 20, Code = "adverts.edit", Name = "Edit Adverts", Category = "Content", Module = "Adverts", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 21, Code = "adverts.delete", Name = "Delete Adverts", Category = "Content", Module = "Adverts", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 22, Code = "adverts.publish", Name = "Publish Adverts", Category = "Content", Module = "Adverts", IsActive = true, CreatedAt = SeedDate },
            
            // Shop
            new() { Id = 23, Code = "products.view", Name = "View Products", Category = "Shop", Module = "Products", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 24, Code = "products.create", Name = "Create Products", Category = "Shop", Module = "Products", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 25, Code = "products.edit", Name = "Edit Products", Category = "Shop", Module = "Products", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 26, Code = "products.delete", Name = "Delete Products", Category = "Shop", Module = "Products", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 27, Code = "products.publish", Name = "Publish Products", Category = "Shop", Module = "Products", IsActive = true, CreatedAt = SeedDate },
            
            // Photos
            new() { Id = 28, Code = "photos.view", Name = "View Photos", Category = "Media", Module = "Photos", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 29, Code = "photos.upload", Name = "Upload Photos", Category = "Media", Module = "Photos", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 30, Code = "photos.delete", Name = "Delete Photos", Category = "Media", Module = "Photos", IsActive = true, CreatedAt = SeedDate },
            
            // Videos
            new() { Id = 31, Code = "videos.view", Name = "View Videos", Category = "Media", Module = "Videos", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 32, Code = "videos.upload", Name = "Upload Videos", Category = "Media", Module = "Videos", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 33, Code = "videos.delete", Name = "Delete Videos", Category = "Media", Module = "Videos", IsActive = true, CreatedAt = SeedDate },
            
            // Forums
            new() { Id = 34, Code = "forums.view", Name = "View Forums", Category = "Community", Module = "Forums", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 35, Code = "forums.manage", Name = "Manage Forums", Category = "Community", Module = "Forums", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 36, Code = "forums.moderate", Name = "Moderate Forums", Category = "Community", Module = "Forums", IsActive = true, CreatedAt = SeedDate },
            
            // Settings
            new() { Id = 37, Code = "settings.view", Name = "View Settings", Category = "System", Module = "Settings", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 38, Code = "settings.edit", Name = "Edit Settings", Category = "System", Module = "Settings", IsActive = true, CreatedAt = SeedDate },
            
            // Audit
            new() { Id = 39, Code = "audit.view", Name = "View Audit Logs", Category = "System", Module = "Audit", IsActive = true, CreatedAt = SeedDate },

            // Maintenance
            new() { Id = 40, Code = "maintenance.cache", Name = "Clear Website Cache & Data", Category = "System", Module = "Maintenance", IsActive = true, CreatedAt = SeedDate },

            // Home page content control
            new() { Id = 41, Code = "content.homepage", Name = "Manage Home Page Content", Category = "Content", Module = "Posts", IsActive = true, CreatedAt = SeedDate },

            // Gallery publishing
            new() { Id = 42, Code = "photos.publish", Name = "Publish Photos", Category = "Media", Module = "Photos", IsActive = true, CreatedAt = SeedDate },
            new() { Id = 43, Code = "videos.publish", Name = "Publish Videos", Category = "Media", Module = "Videos", IsActive = true, CreatedAt = SeedDate }
        };
        builder.Entity<Permission>().HasData(permissions);

        // Seed group permissions - Administrators get all permissions
        var adminPermissions = permissions.Select((p, index) => new GroupPermission
        {
            Id = index + 1,
            UserGroupId = 1, // Administrators
            PermissionId = p.Id,
            IsGranted = true,
            IsActive = true,
            CreatedAt = SeedDate,
            AssignedAt = SeedDate
        }).ToList();
        builder.Entity<GroupPermission>().HasData(adminPermissions);

        // Seed settings
        builder.Entity<Setting>().HasData(
            new Setting { Id = 1, Key = "site.name", Value = "Social Platform", Group = "General", Description = "Site name", IsActive = true, CreatedAt = SeedDate },
            new Setting { Id = 2, Key = "site.description", Value = "A modern social networking platform", Group = "General", Description = "Site description", IsActive = true, CreatedAt = SeedDate },
            new Setting { Id = 3, Key = "site.logo", Value = "/images/logo.png", Group = "General", Description = "Site logo URL", IsActive = true, CreatedAt = SeedDate },
            new Setting { Id = 4, Key = "registration.enabled", Value = "true", Group = "Registration", ValueType = SettingValueType.Boolean, Description = "Allow user registration", IsActive = true, CreatedAt = SeedDate },
            new Setting { Id = 5, Key = "registration.requireApproval", Value = "false", Group = "Registration", ValueType = SettingValueType.Boolean, Description = "Require admin approval for new users", IsActive = true, CreatedAt = SeedDate },
            new Setting { Id = 6, Key = "content.requireReview", Value = "true", Group = "Content", ValueType = SettingValueType.Boolean, Description = "Require review before publishing", IsActive = true, CreatedAt = SeedDate },
            new Setting { Id = 7, Key = "site.cacheVersion", Value = "1", Group = "System", ValueType = SettingValueType.Number, Description = "Website cache version stamp; bumping it forces the website to reload data from the database", IsActive = true, CreatedAt = SeedDate }
        );
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }

            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
