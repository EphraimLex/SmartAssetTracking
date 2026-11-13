namespace AssetTracking.Level1.Models;

// Represents a laptop or desktop computer.
public class ComputerAsset : Asset
{
    protected ComputerAsset()
    {
    }

    public ComputerAsset(
        string assetType,
        string brand,
        string modelName,
        DateTime purchaseDate,
        decimal purchasePriceUsd,
        string officeLocation,
        string serialNumber,
        DateTime warrantyExpirationDate,
        string? employeeUsername = null)
        : base(
            assetType,
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