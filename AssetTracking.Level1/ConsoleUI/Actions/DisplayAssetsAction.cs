using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.Repositories;

namespace AssetTracking.Level1.ConsoleUI.Actions;

// Retrieves all assets and displays them sorted by ID.
// The supplied display implementation determines the table layout.
public class DisplayAssetsAction : IMenuAction
{
    private readonly IAssetRepository _repository;
    private readonly IAssetDisplay _display;

    public DisplayAssetsAction(
        IAssetRepository repository,
        IAssetDisplay display)
    {
        _repository = repository;
        _display = display;
    }

    public string Key => "2";

    public string Description => "Display all assets";

    public void Execute()
    {
        var assets = _repository.GetAll()
            .OrderBy(asset => asset.Id)
            .ToList();

        _display.DisplayAssets(assets);
    }
}