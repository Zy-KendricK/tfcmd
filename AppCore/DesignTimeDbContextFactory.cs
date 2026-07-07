using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AppCore;

/// <summary>
/// Factory for creating ApplicationDbContext at design time for EF Core migrations.
/// This allows migrations to be created without needing a live database connection.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Use a dummy connection string for design-time migration generation
        // The actual connection string will be used at runtime
        var connectionString = "Server=localhost;Port=3306;Database=tfcmd_design;User=root;Password=password;";

        optionsBuilder.UseMySql(connectionString, ServerVersion.Create(8, 0, 0, Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
