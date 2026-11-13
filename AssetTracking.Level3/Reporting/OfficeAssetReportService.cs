using AssetTracking.Level1.Models;
using AssetTracking.Level1.Repositories;
using AssetTracking.Level3.Models;
using AssetTracking.Level3.Repositories;
using AssetTracking.Level3.Services;

namespace AssetTracking.Level3.Reporting;

// Calculates asset counts and purchase-price totals for each office.
public class OfficeAssetReportService : IOfficeAssetReportService
{
    private readonly IAssetRepository _assetRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly OfficeResolver _officeResolver;
    private readonly ICurrencyConverter _currencyConverter;

    public OfficeAssetReportService(
        IAssetRepository assetRepository,
        IOfficeRepository officeRepository,
        OfficeResolver officeResolver,
        ICurrencyConverter currencyConverter)
    {
        _assetRepository = assetRepository;
        _officeRepository = officeRepository;
        _officeResolver = officeResolver;
        _currencyConverter = currencyConverter;
    }

    public OfficeAssetReportResult Generate()
    {
        // Loads each collection once.
        var assets = _assetRepository.GetAll();
        IReadOnlyList<Office> offices = _officeRepository.GetAll();

        // Includes every registered office, even those without assets.
        var assetsByOffice = offices.ToDictionary(
            office => office.Id,
            _ => new List<Asset>());

        List<Asset> unmatchedAssets = new();

        foreach (Asset asset in assets)
        {
            Office? office = _officeResolver.Resolve(
                asset.OfficeLocation,
                offices);

            if (office is null)
            {
                unmatchedAssets.Add(asset);
                continue;
            }

            assetsByOffice[office.Id].Add(asset);
        }

        List<OfficeAssetReport> reports = new();

        foreach (Office office in offices
            .OrderBy(office => office.OfficeName)
            .ThenBy(office => office.Id))
        {
            List<Asset> officeAssets = assetsByOffice[office.Id];

            decimal totalUsd = officeAssets.Sum(
                asset => asset.PurchasePriceUsd);

            string currencyCode =
                office.CurrencyCode.Trim().ToUpperInvariant();

            decimal? totalLocal = CalculateLocalTotal(
                officeAssets,
                currencyCode);

            reports.Add(new OfficeAssetReport(
                office.Id,
                office.OfficeName,
                office.Country,
                currencyCode,
                officeAssets.Count,
                totalUsd,
                totalLocal));
        }

        return new OfficeAssetReportResult(
            reports,
            unmatchedAssets.OrderBy(asset => asset.Id));
    }

    private decimal? CalculateLocalTotal(
        IReadOnlyList<Asset> assets,
        string currencyCode)
    {
        // Matches the currencies supported by the current converter.
        if (currencyCode is not ("USD" or "SEK" or "EUR" or "TRY"))
        {
            return null;
        }

        // Sums individually rounded conversions so the total matches
        // the local prices shown in the asset table.
        return assets.Sum(asset =>
            _currencyConverter.ConvertFromUsd(
                asset.PurchasePriceUsd,
                currencyCode));
    }
}