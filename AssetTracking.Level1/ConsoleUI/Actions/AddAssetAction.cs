using AssetTracking.Level1.Repositories;

namespace AssetTracking.Level1.ConsoleUI.Actions;

// Collects asset details and saves the new asset.
public class AddAssetAction : IMenuAction
{
    private readonly AssetInput _input;
    private readonly IAssetRepository _repository;

    public AddAssetAction(
        AssetInput input,
        IAssetRepository repository)
    {
        _input = input;
        _repository = repository;
    }

    public string Key => "1";

    public string Description => "Add asset";

    public void Execute()
    {
        var asset = _input.ReadAsset();

        _repository.Add(asset);

        Console.WriteLine();
        Console.WriteLine("Asset saved successfully.");
    }
}