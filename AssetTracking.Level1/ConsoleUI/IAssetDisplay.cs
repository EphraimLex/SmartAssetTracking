using AssetTracking.Level1.Models;

namespace AssetTracking.Level1.ConsoleUI;

// Defines the views that each level's asset display must provide.
public interface IAssetDisplay
{
    // Displays all assets in a standard table.
    void DisplayAssets(IReadOnlyList<Asset> assets);

    // Displays assets grouped by category and sorted by purchase date.
    void DisplayByCategory(IReadOnlyList<Asset> assets);
}