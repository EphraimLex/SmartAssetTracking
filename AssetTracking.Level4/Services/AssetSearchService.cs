using AssetTracking.Level1.Models;

namespace AssetTracking.Level4.Services;

// Applies optional search and filter conditions to assets.
public class AssetSearchService
{
    public IReadOnlyList<Asset> Search(
        IEnumerable<Asset> assets,
        string? brand,
        string? model,
        string? office,
        int? purchaseYear,
        string category,
        bool expiredOnly)
    {
        DateTime today = DateTime.Today;

        return assets
            .Where(asset => Matches(asset.Brand, brand))
            .Where(asset => Matches(asset.ModelName, model))
            .Where(asset => Matches(asset.OfficeLocation, office))
            .Where(asset =>
                !purchaseYear.HasValue ||
                asset.PurchaseDate.Year == purchaseYear.Value)
            .Where(asset => category switch
            {
                "1" => asset is ComputerAsset,
                "2" => asset is MobileAsset,
                _ => true
            })
            .Where(asset =>
                !expiredOnly ||
                asset.PurchaseDate.Date.AddYears(3) <= today)
            .OrderBy(asset => asset.OfficeLocation)
            .ThenBy(asset => asset.PurchaseDate)
            .ThenBy(asset => asset.Id)
            .ToList();
    }

    private static bool Matches(string value, string? search)
    {
        return string.IsNullOrWhiteSpace(search) ||
            value.Contains(
                search.Trim(),
                StringComparison.OrdinalIgnoreCase);
    }
}