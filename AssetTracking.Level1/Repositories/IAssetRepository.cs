using AssetTracking.Level1.Models;

namespace AssetTracking.Level1.Repositories;

// Defines asset storage operations shared by the application levels.
public interface IAssetRepository
{
    void Add(Asset asset);

    void AddRange(IEnumerable<Asset> assets);

    IReadOnlyList<Asset> GetAll();

    Asset? GetById(int id);

    void Update(Asset asset);

    bool Delete(int id);
}