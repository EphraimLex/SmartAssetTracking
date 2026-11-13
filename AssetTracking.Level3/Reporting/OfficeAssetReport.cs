namespace AssetTracking.Level3.Reporting;

// Holds calculated asset totals for one registered office.
public class OfficeAssetReport
{
    public int OfficeId { get; }

    public string OfficeName { get; }

    public string Country { get; }

    public string CurrencyCode { get; }

    public int AssetCount { get; }

    public decimal TotalValueUsd { get; }

    // Null means local value could not be calculated.
    // Zero means a valid calculated value of zero.
    public decimal? TotalValueLocal { get; }

    public OfficeAssetReport(
        int officeId,
        string officeName,
        string country,
        string currencyCode,
        int assetCount,
        decimal totalValueUsd,
        decimal? totalValueLocal)
    {
        OfficeId = officeId;
        OfficeName = officeName;
        Country = country;
        CurrencyCode = currencyCode;
        AssetCount = assetCount;
        TotalValueUsd = totalValueUsd;
        TotalValueLocal = totalValueLocal;
    }
}