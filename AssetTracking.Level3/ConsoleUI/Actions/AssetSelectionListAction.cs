using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level1.Repositories;

namespace AssetTracking.Level3.ConsoleUI.Actions;

// Shows selectable assets before running an existing menu action.
public class AssetSelectionListAction : IMenuAction
{
    private readonly IMenuAction _action;
    private readonly IAssetRepository _repository;

    public AssetSelectionListAction(
        IMenuAction action,
        IAssetRepository repository)
    {
        _action = action;
        _repository = repository;
    }

    // Preserves the wrapped action's menu option.
    public string Key => _action.Key;

    public string Description => _action.Description;

    public void Execute()
    {
        var assets = _repository.GetAll()
            .OrderBy(asset => asset.Id)
            .ToList();

        Console.WriteLine();

        if (assets.Count == 0)
        {
            Console.WriteLine("No assets found.");
            return;
        }

        Console.WriteLine("Available assets");
        Console.WriteLine("----------------");

        foreach (var asset in assets)
        {
            Console.WriteLine(
                $"{asset.Id}: {asset.Brand} {asset.ModelName} " +
                $"| {asset.OfficeLocation} " +
                $"| Serial: {asset.SerialNumber}");
        }

        // The existing action handles ID input and cancellation.
        _action.Execute();
    }
}