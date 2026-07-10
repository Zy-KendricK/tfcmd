using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AppCore;

/// <summary>
/// Factory for creating ApplicationDbContext at design time for EF Core migrations.
/// Reads the real connection string from the Admin project's appsettings.json when available
/// so commands like 'dotnet ef migrations list' and 'database update' hit the correct database.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var connectionString = ResolveConnectionString()
            ?? "Server=localhost;Port=3306;Database=tfcmd_design;User=root;Password=password;";

        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.Create(8, 0, 0, Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql),
            mySqlOptions => mySqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null));

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string? ResolveConnectionString()
    {
        // Walk up from the current directory looking for Admin/appsettings.json
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, "Admin", "appsettings.json");
            if (File.Exists(candidate))
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile(candidate, optional: true)
                    .Build();
                var cs = config.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrWhiteSpace(cs))
                {
                    return cs;
                }
            }

            dir = dir.Parent;
        }

        return null;
    }
}
