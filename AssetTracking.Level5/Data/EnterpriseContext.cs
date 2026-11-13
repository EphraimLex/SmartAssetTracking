using AssetTracking.Level5.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetTracking.Level5.Data;

// Stores Level 5 data in the existing asset_tracking database.
// Earlier contexts retain ownership of asset and office tables.
public class EnterpriseContext : DbContext
{
    public EnterpriseContext(
        DbContextOptions<EnterpriseContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<MaintenanceRecord> MaintenanceRecords =>
        Set<MaintenanceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(employee =>
        {
            employee.ToTable("Employees");

            employee.HasKey(value => value.Id);

            employee.Property(value => value.Username)
                .HasMaxLength(100)
                .IsRequired();

            employee.HasIndex(value => value.Username)
                .IsUnique();

            employee.Property(value => value.FullName)
                .HasMaxLength(150)
                .IsRequired();

            employee.Property(value => value.Department)
                .HasMaxLength(100)
                .IsRequired();

            employee.Property(value => value.Email)
                .HasMaxLength(254)
                .IsRequired();

            employee.Property(value => value.Role)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
        });

        modelBuilder.Entity<MaintenanceRecord>(maintenance =>
        {
            maintenance.ToTable("MaintenanceRecords");

            maintenance.HasKey(value => value.Id);

            maintenance.HasIndex(value => value.AssetId);

            maintenance.Property(value => value.LastMaintenanceDate)
                .HasColumnType("date");

            maintenance.Property(value => value.NextMaintenanceDate)
                .HasColumnType("date");

            maintenance.Property(value => value.Notes)
                .HasMaxLength(2000)
                .IsRequired();
        });
    }
}