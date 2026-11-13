namespace AssetTracking.Level1.Models;

// Represents a mobile phone with common properties inherited from Asset.
public class MobileAsset : Asset
{
    protected MobileAsset()
    {
    }

    public MobileAsset(
        string brand,
        string modelName,
        DateTime purchaseDate,
        decimal purchasePriceUsd,
        string officeLocation,
        string serialNumber,
        DateTime warrantyExpirationDate,
        string? employeeUsername = null)
        : base(
            "Mobile Phone",
            brand,
            modelName,
            purchaseDate,
            purchasePriceUsd,
            officeLocation,
            serialNumber,
            warrantyExpirationDate,
            employeeUsername)
    {
    }
}