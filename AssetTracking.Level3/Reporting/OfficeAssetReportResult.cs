using AssetTracking.Level1.Models;

namespace AssetTracking.Level3.Reporting;

// Holds office summaries and assets without a unique office match.
public class OfficeAssetReportResult
{
    public IReadOnlyList<OfficeAssetReport> Offices { get; }

    public IReadOnlyList<Asset> UnmatchedAssets { get; }

    public OfficeAssetReportResult(
        IEnumerable<OfficeAssetReport> offices,
        IEnumerable<Asset> unmatchedAssets)
    {
        // Copies the collections so callers cannot add or remove entries.
        Offices = offices.ToList().AsReadOnly();
        UnmatchedAssets = unmatchedAssets.ToList().AsReadOnly();
    }
}