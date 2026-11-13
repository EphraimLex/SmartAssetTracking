namespace AssetTracking.Level1.Models;

// Holds the properties shared by all assets.
public abstract class Asset
{
    public int Id { get; set; }

    public string AssetType { get; set; } = "";

    public string Brand { get; set; } = "";

    public string ModelName { get; set; } = "";

    public DateTime PurchaseDate { get; set; }

    public decimal PurchasePriceUsd { get; set; }

    public string OfficeLocation { get; set; } = "";

    public string SerialNumber { get; set; } = "";

    // Nullable because assigning an employee is optional.
    public string? EmployeeUsername { get; set; }

    public DateTime WarrantyExpirationDate { get; set; }

    // Allows derived classes and EF Core to construct an asset.
    protected Asset()
    {
    }

    protected Asset(
        string assetType,
        string brand,
        string modelName,
        DateTime purchaseDate,
        decimal purchasePriceUsd,
        string officeLocation,
        string serialNumber,
        DateTime warrantyExpirationDate,
        string? employeeUsername = null)
    {
        AssetType = assetType;
        Brand = brand;
        ModelName = modelName;
        PurchaseDate = purchaseDate;
        PurchasePriceUsd = purchasePriceUsd;
        OfficeLocation = officeLocation;
        SerialNumber = serialNumber;
        WarrantyExpirationDate = warrantyExpirationDate;
        EmployeeUsername = employeeUsername;
    }
}