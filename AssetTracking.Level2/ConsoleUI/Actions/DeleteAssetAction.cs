using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level1.Repositories;

namespace AssetTracking.Level2.ConsoleUI.Actions;

// Deletes an existing asset after the user confirms.
public class DeleteAssetAction : IMenuAction
{
    private readonly AssetInput _input;
    private readonly IAssetRepository _repository;

    public DeleteAssetAction(
        AssetInput input,
        IAssetRepository repository)
    {
        _input = input;
        _repository = repository;
    }

    public string Key => "4";

    public string Description => "Delete asset";

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

        Console.WriteLine();
        Console.WriteLine(
            $"Delete asset {asset.Id}: " +
            $"{asset.Brand} {asset.ModelName} ({asset.AssetType})?");

        Console.Write("Type YES to confirm: ");

        string confirmation = Console.ReadLine()
            ?? throw new OperationCanceledException();

        if (!string.Equals(
            confirmation.Trim(),
            "YES",
            StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Deletion cancelled.");
            return;
        }

        bool deleted = _repository.Delete(id);

        Console.WriteLine(
            deleted
                ? "Asset deleted successfully."
                : "Asset not found.");
    }
}