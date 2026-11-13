using System.Text.Json;
using AssetTracking.Level3.Models;

namespace AssetTracking.Level3.Importing;

// Reads and validates offices from a JSON file.
// Database saving is handled separately by the repository.
public class OfficeFileReader
{
    public IReadOnlyList<Office> Read(string filePath)
    {
        string json = File.ReadAllText(filePath);

        using JsonDocument document = JsonDocument.Parse(json);

        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            throw new FormatException(
                "The JSON file must contain an array of offices.");
        }

        List<Office> offices = new();

        foreach (JsonElement item in document.RootElement.EnumerateArray())
        {
            int entryNumber = offices.Count + 1;

            try
            {
                offices.Add(ReadOffice(item));
            }
            catch (FormatException exception)
            {
                throw new FormatException(
                    $"Office entry {entryNumber}: {exception.Message}",
                    exception);
            }
        }

        if (offices.Count == 0)
        {
            throw new FormatException("The file contains no offices.");
        }

        return offices;
    }

    private static Office ReadOffice(JsonElement item)
    {
        if (item.ValueKind != JsonValueKind.Object)
        {
            throw new FormatException(
                "Each office must be a JSON object.");
        }

        string officeName = ReadRequiredText(item, "OfficeName", 100);
        string country = ReadRequiredText(item, "Country", 100);

        string currencyCode = ReadRequiredText(
            item, "CurrencyCode", 3).ToUpperInvariant();

        if (currencyCode is not ("SEK" or "USD" or "EUR" or "TRY"))
        {
            throw new FormatException(
                "CurrencyCode must be SEK, USD, EUR, or TRY.");
        }

        // The database generates the ID when the office is saved.
        return new Office(officeName, country, currencyCode);
    }

    private static string ReadRequiredText(
        JsonElement item,
        string propertyName,
        int maxLength)
    {
        // Accepts both PascalCase and camelCase property names.
        // Rejects ambiguous entries such as OfficeName and officeName
        // appearing together in the same object.
        JsonElement property = default;
        bool found = false;

        foreach (JsonProperty candidate in item.EnumerateObject())
        {
            if (!string.Equals(
                candidate.Name,
                propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (found)
            {
                throw new FormatException(
                    $"{propertyName} must appear only once.");
            }

            property = candidate.Value;
            found = true;
        }

        if (!found || property.ValueKind == JsonValueKind.Null)
        {
            throw new FormatException($"{propertyName} is required.");
        }

        if (property.ValueKind != JsonValueKind.String)
        {
            throw new FormatException($"{propertyName} must be text.");
        }

        string value = property.GetString()?.Trim() ?? "";

        if (value.Length == 0)
        {
            throw new FormatException($"{propertyName} is required.");
        }

        if (value.Length > maxLength)
        {
            throw new FormatException(
                $"{propertyName} must not exceed {maxLength} characters.");
        }

        return value;
    }
}