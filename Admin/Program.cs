using Admin.Infrastructure;
using Admin.Logging;
using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=SocialPlatform;User=root;Password=;";

// Use a fixed MySQL version to avoid connection during DI
var serverVersion = ServerVersion.Create(8, 0, 0, ServerType.MySql);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion, mySqlOptions =>
        mySqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null)));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure authentication cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

// Add services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IUserGroupService, UserGroupService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IHomeContentService, HomeContentService>();
builder.Services.AddScoped<Admin.Services.IImageUploadService, Admin.Services.ImageUploadService>();

var logPath = Path.Combine(builder.Environment.ContentRootPath, "Logs", "system.log");
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddProvider(new FileLoggerProvider(logPath));

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AuditLoggingFilter>();
});

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Initialize database and seed super admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        // Check if super admin exists, if not create one
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var superAdmin = await context.Users.FirstOrDefaultAsync(u => u.IsSuperAdmin);

        if (superAdmin == null)
        {
            var user = new ApplicationUser
            {
                UserName = "admin@platform.com",
                Email = "admin@platform.com",
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Administrator",
                DisplayName = "System Administrator",
                IsSuperAdmin = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, "Admin@123");
            if (result.Succeeded)
            {
                // Add to Administrators group
                var adminGroup = await context.UserGroups.FirstOrDefaultAsync(g => g.Name == "Administrators");
                if (adminGroup != null)
                {
                    context.UserGroupMemberships.Add(new UserGroupMembership
                    {
                        UserId = user.Id,
                        UserGroupId = adminGroup.Id,
                        IsPrimaryGroup = true,
                        IsActive = true,
                        JoinedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    });
                    await context.SaveChangesAsync();
                }
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
