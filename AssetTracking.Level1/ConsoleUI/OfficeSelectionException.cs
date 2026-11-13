namespace AssetTracking.Level1.ConsoleUI;

// Indicates that an office cannot be selected for the current action.
public class OfficeSelectionException : Exception
{
    public OfficeSelectionException(string message)
        : base(message)
    {
    }
}