using AssetTracking.Level1.Repositories;
using AssetTracking.Level5.Data;
using AssetTracking.Level5.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetTracking.Level5.Services;

// Validates and stores maintenance records for existing assets.
public class MaintenanceService
{
    private readonly EnterpriseContext _context;
    private readonly IAssetRepository _assets;

    public MaintenanceService(
        EnterpriseContext context,
        IAssetRepository assets)
    {
        _context = context;
        _assets = assets;
    }

    public void Add(
        int assetId,
        DateTime lastMaintenanceDate,
        DateTime nextMaintenanceDate,
        string notes)
    {
        var asset = _assets.GetById(assetId)
            ?? throw new ArgumentException("Asset not found.");

        if (lastMaintenanceDate.Date < asset.PurchaseDate.Date)
        {
            throw new ArgumentException(
                "Maintenance cannot be before the purchase date.");
        }

        if (lastMaintenanceDate.Date > DateTime.Today)
        {
            throw new ArgumentException(
                "Completed maintenance cannot be in the future.");
        }

        if (nextMaintenanceDate.Date <= lastMaintenanceDate.Date)
        {
            throw new ArgumentException(
                "Next maintenance must be after the last maintenance.");
        }

        notes = notes.Trim();

        if (notes.Length == 0 || notes.Length > 2000)
        {
            throw new ArgumentException(
                "Maintenance notes must contain 1–2000 characters.");
        }

        MaintenanceRecord record = new()
        {
            AssetId = assetId,
            LastMaintenanceDate = lastMaintenanceDate.Date,
            NextMaintenanceDate = nextMaintenanceDate.Date,
            Notes = notes
        };

        _context.MaintenanceRecords.Add(record);
        _context.SaveChanges();
    }

    public IReadOnlyList<MaintenanceRecord> GetForAsset(int assetId)
    {
        return _context.MaintenanceRecords
            .AsNoTracking()
            .Where(record => record.AssetId == assetId)
            .OrderByDescending(record => record.LastMaintenanceDate)
            .ThenByDescending(record => record.Id)
            .ToList();
    }
}