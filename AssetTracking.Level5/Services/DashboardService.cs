using AssetTracking.Level1.Repositories;
using AssetTracking.Level3.Repositories;
using AssetTracking.Level3.Services;

namespace AssetTracking.Level5.Services;

// Calculates dashboard statistics independently of console output.
public class DashboardService
{
    private readonly IAssetRepository _assets;
    private readonly EmployeeService _employees;
    private readonly IOfficeRepository _offices;
    private readonly OfficeResolver _officeResolver;

    public DashboardService(
        IAssetRepository assets,
        EmployeeService employees,
        IOfficeRepository offices,
        OfficeResolver officeResolver)
    {
        _assets = assets;
        _employees = employees;
        _offices = offices;
        _officeResolver = officeResolver;
    }

    public DashboardSummary GetSummary()
    {
        var assets = _assets.GetAll();
        var employees = _employees.GetAll();
        var offices = _offices.GetAll();

        DateTime today = DateTime.Today;
        DateTime warningDate = today.AddMonths(6);

        // Counts registered assets by their stored asset type.
        var typeCounts = assets
            .GroupBy(asset => asset.AssetType)
            .Select(group => new
            {
                Type = group.Key,
                Count = group.Count()
            })
            .ToList();

        int highestCount = typeCounts.Count == 0
            ? 0
            : typeCounts.Max(item => item.Count);

        // Includes all types tied for the highest count.
        string mostUsedTypes = highestCount == 0
            ? "None"
            : string.Join(
                ", ",
                typeCounts
                    .Where(item => item.Count == highestCount)
                    .OrderBy(item => item.Type)
                    .Select(item => $"{item.Type} ({item.Count})"));

        return new DashboardSummary(
            TotalAssets: assets.Count,

            TotalValueUsd: assets.Sum(
                asset => asset.PurchasePriceUsd),

            ExpiredAssets: assets.Count(
                asset => asset.PurchaseDate.Date.AddYears(3) <= today),

            ExpiringAssets: assets.Count(asset =>
                asset.PurchaseDate.Date.AddYears(3) > today
                && asset.PurchaseDate.Date.AddYears(3) < warningDate),

            UnassignedAssets: assets.Count(
                asset => string.IsNullOrWhiteSpace(
                    asset.EmployeeUsername)),

            MostUsedTypes: mostUsedTypes,

            // Groups matching labels under the registered office name.
            // Preserves the original label if resolution is unsuccessful.
            OfficeValues: assets
                .GroupBy(
                    asset => _officeResolver.Resolve(
                        asset.OfficeLocation,
                        offices)?.OfficeName
                            ?? asset.OfficeLocation.Trim(),
                    StringComparer.OrdinalIgnoreCase)
                .OrderBy(group => group.Key)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(
                        asset => asset.PurchasePriceUsd)),

            EmployeeAssetCounts: employees
                .ToDictionary(
                    employee => employee.Username,
                    employee => assets.Count(asset =>
                        string.Equals(
                            asset.EmployeeUsername,
                            employee.Username,
                            StringComparison.OrdinalIgnoreCase)))
        );
    }
}

// Carries calculated values to the display action.
public record DashboardSummary(
    int TotalAssets,
    decimal TotalValueUsd,
    int ExpiredAssets,
    int ExpiringAssets,
    int UnassignedAssets,
    string MostUsedTypes,
    IReadOnlyDictionary<string, decimal> OfficeValues,
    IReadOnlyDictionary<string, int> EmployeeAssetCounts);