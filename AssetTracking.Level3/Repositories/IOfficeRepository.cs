using AssetTracking.Level3.Models;

namespace AssetTracking.Level3.Repositories;

// Defines the database operations available for company offices.
public interface IOfficeRepository
{
    // Returns all offices.
    IReadOnlyList<Office> GetAll();

    // Returns the matching office, or null if it does not exist.
    Office? GetById(int id);

    // Saves a new office.
    void Add(Office office);

    // Saves multiple offices together.
    void AddRange(IEnumerable<Office> offices);
}