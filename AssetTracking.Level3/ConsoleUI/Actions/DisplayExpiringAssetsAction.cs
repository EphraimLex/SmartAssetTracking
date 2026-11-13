using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level1.Repositories;

namespace AssetTracking.Level3.ConsoleUI.Actions;

// Displays assets nearing or past the end of their three-year lifetime.
public class DisplayExpiringAssetsAction : IMenuAction
{
    private readonly IAssetRepository _repository;
    private readonly IAssetDisplay _display;

    public DisplayExpiringAssetsAction(
        IAssetRepository repository,
        IAssetDisplay display)
    {
        _repository = repository;
        _display = display;
    }

    public string Key => "11";

    public string Description => "Display expiring and expired assets";

    public void Execute()
    {
        DateTime cutoff = DateTime.Today.AddMonths(6);

        var assets = _repository.GetAll()
            .Where(asset =>
                asset.PurchaseDate.Date.AddYears(3) < cutoff)
            .ToList();

        Console.WriteLine();
        Console.WriteLine(
            "Assets with less than six months of lifetime remaining, " +
            "including expired assets.");

        if (assets.Count == 0)
        {
            Console.WriteLine("No assets match this condition.");
            return;
        }

        _display.DisplayAssets(assets);
    }
}