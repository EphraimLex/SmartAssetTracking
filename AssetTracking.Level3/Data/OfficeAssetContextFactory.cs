using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AssetTracking.Level3.Data;

// Creates the Level 3 context for EF Core migration commands.
public class OfficeAssetContextFactory
    : IDesignTimeDbContextFactory<OfficeAssetContext>
{
    public OfficeAssetContext CreateDbContext(string[] args)
    {
        string connectionString =
            @"Server=localhost\SQLEXPRESS;" +
            "Database=asset_tracking;" +
            "Integrated Security=True;" +
            "TrustServerCertificate=True;";

        var options = new DbContextOptionsBuilder<OfficeAssetContext>()
            .UseSqlServer(
                connectionString,
                sqlOptions =>
                {
                    // Tracks Level 3 migrations separately from Level 1.
                    sqlOptions.MigrationsHistoryTable(
                        "__EFMigrationsHistory_Level3");
                })
            .Options;

        return new OfficeAssetContext(options);
    }
}