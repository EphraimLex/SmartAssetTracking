namespace AssetTracking.Level1.ConsoleUI;

// Signals that the user cancelled the current menu action.
public class ActionCancelledException : Exception
{
    public ActionCancelledException(string message)
        : base(message)
    {
    }
}