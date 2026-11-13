namespace AssetTracking.Level3.Models;

// Represents a company office and its local currency.
public class Office
{
    public int Id { get; set; }

    public string OfficeName { get; set; } = "";

    public string Country { get; set; } = "";

    // Uses standard currency codes, such as SEK, USD, EUR, or TRY.
    public string CurrencyCode { get; set; } = "";

    // Allows EF Core to create an instance when loading database records.
    public Office()
    {
    }

    // Creates an office with its required details.
    public Office(
        string officeName,
        string country,
        string currencyCode)
    {
        OfficeName = officeName;
        Country = country;
        CurrencyCode = currencyCode;
    }
}