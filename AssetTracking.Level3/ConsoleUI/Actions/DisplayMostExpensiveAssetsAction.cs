using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level1.Repositories;

namespace AssetTracking.Level3.ConsoleUI.Actions;

// Displays the five assets with the highest USD purchase prices.
public class DisplayMostExpensiveAssetsAction : IMenuAction
{
    private readonly IAssetRepository _repository;
    private readonly IAssetDisplay _display;

    public DisplayMostExpensiveAssetsAction(
        IAssetRepository repository,
        IAssetDisplay display)
    {
        _repository = repository;
        _display = display;
    }

    public string Key => "12";

    public string Description => "Display most expensive assets";

    public void Execute()
    {
        const int maximumResults = 5;

        var assets = _repository.GetAll()
            .OrderByDescending(asset => asset.PurchasePriceUsd)
            .ThenBy(asset => asset.Id)
            .Take(maximumResults)
            .ToList();

        Console.WriteLine();
        Console.WriteLine(
            $"Top {maximumResults} assets by USD purchase price.");

        if (assets.Count == 0)
        {
            Console.WriteLine("No assets found.");
            return;
        }

        _display.DisplayAssets(assets);
    }
}