using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AssetTracking.Level1.Data;

// Creates the context for EF Core migration commands.
public class AssetContextFactory
    : IDesignTimeDbContextFactory<AssetContext>
{
    public AssetContext CreateDbContext(string[] args)
    {
        string connectionString =
            @"Server=localhost\SQLEXPRESS;" +
            "Database=asset_tracking;" +
            "Integrated Security=True;" +
            "TrustServerCertificate=True;";

        var options = new DbContextOptionsBuilder<AssetContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AssetContext(options);
    }
}