using AssetTracking.Level1.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetTracking.Level1.Data;

// Connects the asset models to the database through EF Core.
public class AssetContext : DbContext
{
    // Used when creating AssetContext directly in Level 1 or Level 2.
    public AssetContext(
        DbContextOptions<AssetContext> options)
        : base(options)
    {
    }

    // Allows a derived context to pass its own database configuration.
    // The non-generic type accepts options for the derived context.
    protected AssetContext(DbContextOptions options)
        : base(options)
    {
    }

    // Provides access to all assets, including computers and mobiles.
    public DbSet<Asset> Assets => Set<Asset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Registers both derived asset types in the EF Core model.
        modelBuilder.Entity<ComputerAsset>();
        modelBuilder.Entity<MobileAsset>();

        // Stores USD prices with two decimal places.
        modelBuilder.Entity<Asset>()
            .Property(asset => asset.PurchasePriceUsd)
            .HasPrecision(18, 2);
    }
}