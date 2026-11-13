namespace AssetTracking.Level1.ConsoleUI;

// Defines how an office location is obtained for an asset.
// Each level can provide its own input implementation.
public interface IOfficeLocationInput
{
    string ReadOfficeLocation();
}