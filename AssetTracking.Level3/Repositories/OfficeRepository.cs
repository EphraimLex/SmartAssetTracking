using AssetTracking.Level3.Data;
using AssetTracking.Level3.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetTracking.Level3.Repositories;

// Handles office database operations using Entity Framework Core.
public class OfficeRepository : IOfficeRepository
{
    private readonly OfficeAssetContext _context;

    public OfficeRepository(OfficeAssetContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Office> GetAll()
    {
        // Tracking is unnecessary when displaying office information.
        return _context.Offices
            .AsNoTracking()
            .OrderBy(office => office.OfficeName)
            .ThenBy(office => office.Id)
            .ToList();
    }

    public Office? GetById(int id)
    {
        return _context.Offices
            .AsNoTracking()
            .FirstOrDefault(office => office.Id == id);
    }

    public void Add(Office office)
    {
        _context.Offices.Add(office);
        _context.SaveChanges();
    }

    // Saves the imported offices in one database operation.
    public void AddRange(IEnumerable<Office> offices)
    {
        _context.Offices.AddRange(offices);
        _context.SaveChanges();
    }
}