using System.Text.Json;
using AssetTracking.Level1.Models;

namespace AssetTracking.Level2.Importing;

// Reads and validates assets from a JSON file.
// Database saving is handled separately by the repository.
public class AssetFileReader
{
    public IReadOnlyList<Asset> Read(string filePath)
    {
        string json = File.ReadAllText(filePath);

        using JsonDocument document = JsonDocument.Parse(json);

        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            throw new FormatException(
                "The JSON file must contain an array of assets.");
        }

        List<Asset> assets = new();

        foreach (JsonElement item in document.RootElement.EnumerateArray())
        {
            int entryNumber = assets.Count + 1;

            try
            {
                assets.Add(ReadAsset(item));
            }
            catch (FormatException exception)
            {
                throw new FormatException(
                    $"Asset entry {entryNumber}: {exception.Message}",
                    exception);
            }
        }

        if (assets.Count == 0)
        {
            throw new FormatException("The file contains no assets.");
        }

        return assets;
    }

    private static Asset ReadAsset(JsonElement item)
    {
        if (item.ValueKind != JsonValueKind.Object)
        {
            throw new FormatException(
                "Each asset must be a JSON object.");
        }

        string assetType = ReadRequiredText(item, "AssetType");
        string brand = ReadRequiredText(item, "Brand");
        string modelName = ReadRequiredText(item, "ModelName");
        string officeLocation = ReadRequiredText(item, "OfficeLocation");
        string serialNumber = ReadRequiredText(item, "SerialNumber");

        DateTime purchaseDate = ReadDate(item, "PurchaseDate");

        DateTime warrantyExpirationDate = ReadDate(
            item, "WarrantyExpirationDate");

        if (warrantyExpirationDate < purchaseDate)
        {
            throw new FormatException(
                "Warranty expiration cannot be before the purchase date.");
        }

        if (!item.TryGetProperty("PurchasePriceUsd", out JsonElement price)
            || price.ValueKind != JsonValueKind.Number
            || !price.TryGetDecimal(out decimal purchasePriceUsd)
            || purchasePriceUsd < 0)
        {
            throw new FormatException(
                "PurchasePriceUsd must be a non-negative number.");
        }

        string? employeeUsername = ReadOptionalText(
            item, "EmployeeUsername");

        // Constructors create the correct entity class.
        // IDs are omitted so the database generates them.
        return assetType switch
        {
            "Laptop" or "Desktop" => new ComputerAsset(
                assetType,
                brand,
                modelName,
                purchaseDate,
                purchasePriceUsd,
                officeLocation,
                serialNumber,
                warrantyExpirationDate,
                employeeUsername),

            "Mobile Phone" => new MobileAsset(
                brand,
                modelName,
                purchaseDate,
                purchasePriceUsd,
                officeLocation,
                serialNumber,
                warrantyExpirationDate,
                employeeUsername),

            _ => throw new FormatException(
                "AssetType must be Laptop, Desktop, or Mobile Phone.")
        };
    }

    private static string ReadRequiredText(
        JsonElement item,
        string propertyName)
    {
        string? value = ReadOptionalText(item, propertyName);

        if (value is null)
        {
            throw new FormatException(
                $"{propertyName} is required.");
        }

        return value;
    }

    private static string? ReadOptionalText(
        JsonElement item,
        string propertyName)
    {
        if (!item.TryGetProperty(propertyName, out JsonElement property)
            || property.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (property.ValueKind != JsonValueKind.String)
        {
            throw new FormatException(
                $"{propertyName} must be text.");
        }

        string? value = property.GetString()?.Trim();

        return string.IsNullOrEmpty(value) ? null : value;
    }

    private static DateTime ReadDate(
        JsonElement item,
        string propertyName)
    {
        string value = ReadRequiredText(item, propertyName);

        if (DateTime.TryParseExact(
            value,
            "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out DateTime date))
        {
            return date;
        }

        throw new FormatException(
            $"{propertyName} must be a valid date in yyyy-MM-dd format.");
    }
}