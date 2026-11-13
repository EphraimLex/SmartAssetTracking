using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level1.Repositories;

namespace AssetTracking.Level2.ConsoleUI.Actions;

// Retrieves assets and requests the current level's category view.
public class DisplayAssetsByCategoryAction : IMenuAction
{
    private readonly IAssetRepository _repository;
    private readonly IAssetDisplay _display;

    public DisplayAssetsByCategoryAction(
        IAssetRepository repository,
        IAssetDisplay display)
    {
        _repository = repository;
        _display = display;
    }

    public string Key => "6";

    public string Description => "Display assets by category";

    public void Execute()
    {
        var assets = _repository.GetAll();

        _display.DisplayByCategory(assets);
    }
}