using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level1.Repositories;

namespace AssetTracking.Level2.ConsoleUI.Actions;

// Collects replacement details and saves changes to an existing asset.
public class UpdateAssetAction : IMenuAction
{
    private readonly AssetInput _input;
    private readonly IAssetRepository _repository;

    public UpdateAssetAction(
        AssetInput input,
        IAssetRepository repository)
    {
        _input = input;
        _repository = repository;
    }

    public string Key => "3";

    public string Description => "Update asset";

    public void Execute()
    {
        Console.WriteLine();

        int id = _input.ReadId();

        var asset = _repository.GetById(id);

        if (asset is null)
        {
            Console.WriteLine("Asset not found.");
            return;
        }

        _input.ReadChanges(asset);

        _repository.Update(asset);

        Console.WriteLine();
        Console.WriteLine("Asset updated successfully.");
    }
}