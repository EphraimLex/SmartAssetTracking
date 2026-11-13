namespace AssetTracking.Level3.Reporting;

// Defines the operation for generating asset totals by office.
public interface IOfficeAssetReportService
{
    OfficeAssetReportResult Generate();
}