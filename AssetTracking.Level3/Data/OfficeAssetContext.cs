using AssetTracking.Level1.Data;
using AssetTracking.Level1.Models;
using AssetTracking.Level3.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetTracking.Level3.Data;

// Extends the existing asset context with company offices.
public class OfficeAssetContext : AssetContext
{
    public OfficeAssetContext(
        // Holds database configuration for this Level 3 context.
        DbContextOptions<OfficeAssetContext> options)
        // Passes the configuration to AssetContext.
        : base(options)
    {
    }

    // Provides access to the offices stored in the database.
    public DbSet<Office> Offices => Set<Office>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Keeps the asset configuration inherited from Level 1.
        base.OnModelCreating(modelBuilder);

        // Allows asset operations, but leaves changes to the shared
        // Assets table structure under Level 1's migrations.
        modelBuilder.Entity<Asset>()
            .ToTable("Assets", table => table.ExcludeFromMigrations());

        // Configures the office fields.
        modelBuilder.Entity<Office>(office =>
        {
            office.Property(value => value.OfficeName)
                .HasMaxLength(100)
                .IsRequired();

            office.Property(value => value.Country)
                .HasMaxLength(100)
                .IsRequired();

            office.Property(value => value.CurrencyCode)
                .HasMaxLength(3)
                .IsRequired();
        });
    }
}