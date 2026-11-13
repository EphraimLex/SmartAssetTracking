using AssetTracking.Level1.Data;
using AssetTracking.Level1.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetTracking.Level1.Repositories;

// Stores, retrieves, updates, and deletes assets using Entity Framework Core.
public class AssetRepository : IAssetRepository
{
    private readonly AssetContext _context;

    public AssetRepository(AssetContext context)
    {
        _context = context;
    }

    public void Add(Asset asset)
    {
        _context.Assets.Add(asset);
        _context.SaveChanges();
    }

    // Adds all supplied assets and saves them in one database operation.
    public void AddRange(IEnumerable<Asset> assets)
    {
        _context.Assets.AddRange(assets);
        _context.SaveChanges();
    }

    public IReadOnlyList<Asset> GetAll()
    {
        // Tracking is unnecessary when assets are only displayed.
        return _context.Assets
            .AsNoTracking()
            .ToList();
    }

    public Asset? GetById(int id)
    {
        // Finds an asset by its primary key.
        // The returned asset is tracked, allowing its properties to be edited.
        return _context.Assets.Find(id);
    }

    public void Update(Asset asset)
    {
        // Marks the asset for updating and saves its current property values.
        _context.Assets.Update(asset);
        _context.SaveChanges();
    }

    public bool Delete(int id)
    {
        Asset? asset = _context.Assets.Find(id);

        if (asset is null)
        {
            return false;
        }

        _context.Assets.Remove(asset);
        _context.SaveChanges();

        return true;
    }
}