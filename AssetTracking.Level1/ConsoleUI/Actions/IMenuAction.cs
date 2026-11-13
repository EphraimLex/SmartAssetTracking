namespace AssetTracking.Level1.ConsoleUI.Actions;

// Defines an operation that can be selected from the console menu.
public interface IMenuAction
{
    string Key { get; }

    string Description { get; }

    void Execute();
}