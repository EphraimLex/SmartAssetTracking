using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AssetTracking.Level5.Data;

// Creates the Level 5 context when EF Core runs migration commands.
public class EnterpriseContextFactory
    : IDesignTimeDbContextFactory<EnterpriseContext>
{
    public EnterpriseContext CreateDbContext(string[] args)
    {
        string connectionString =
            @"Server=localhost\SQLEXPRESS;" +
            "Database=asset_tracking;" +
            "Integrated Security=True;" +
            "TrustServerCertificate=True;";

        var options = new DbContextOptionsBuilder<EnterpriseContext>()
            .UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.MigrationsHistoryTable(
                    "__EFMigrationsHistory_Level5"))
            .Options;

        return new EnterpriseContext(options);
    }
}