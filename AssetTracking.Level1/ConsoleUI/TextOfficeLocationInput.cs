namespace AssetTracking.Level1.ConsoleUI;

// Collects an office location as free text for Levels 1 and 2.
public class TextOfficeLocationInput : IOfficeLocationInput
{
    private readonly ConsoleInput _input;

    public TextOfficeLocationInput(ConsoleInput input)
    {
        _input = input;
    }

    public string ReadOfficeLocation()
    {
        return _input.ReadRequiredText("Office location: ");
    }
}