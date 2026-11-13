using System.Globalization;
using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.Models;
using AssetTracking.Level3.Models;
using AssetTracking.Level3.Repositories;
using AssetTracking.Level3.Services;

namespace AssetTracking.Level3.ConsoleUI;

// Displays Level 3 assets with office and currency information.
public class OfficeAssetDisplay : IAssetDisplay
{
    private readonly IOfficeRepository _officeRepository;
    private readonly OfficeResolver _officeResolver;
    private readonly ICurrencyConverter _currencyConverter;

    public OfficeAssetDisplay(
        IOfficeRepository officeRepository,
        OfficeResolver officeResolver,
        ICurrencyConverter currencyConverter)
    {
        _officeRepository = officeRepository;
        _officeResolver = officeResolver;
        _currencyConverter = currencyConverter;
    }

    public void DisplayAssets(IReadOnlyList<Asset> assets)
    {
        DisplayTable(assets, groupByCategory: false);
    }

    public void DisplayByCategory(IReadOnlyList<Asset> assets)
    {
        DisplayTable(assets, groupByCategory: true);
    }

    private void DisplayTable(
        IReadOnlyList<Asset> assets,
        bool groupByCategory)
    {
        Console.WriteLine();

        if (assets.Count == 0)
        {
            Console.WriteLine("No assets found.");
            return;
        }

        DateTime today = DateTime.Today;
        IReadOnlyList<Office> offices = _officeRepository.GetAll();

        var rows = assets
            .Select(asset => CreateRow(asset, offices))
            .ToList();

        string[] headings =
        [
            "Type",
            "Brand",
            "Model",
            "Office",
            "Purchase Date",
            "Price in USD",
            "Currency",
            "Local Price"
        ];

        // Uses consistent widths across the complete table.
        int[] widths = headings
            .Select((heading, index) => Math.Max(
                heading.Length,
                rows.Max(row => row.Values[index].Length)))
            .ToArray();

        // Right-aligns prices and left-aligns other columns.
        string FormatLine(string[] values)
        {
            return string.Join(
                "  ",
                values.Select((value, index) =>
                    index is 5 or 7
                        ? value.PadLeft(widths[index])
                        : value.PadRight(widths[index])));
        }

        Console.WriteLine(FormatLine(headings));

        Console.WriteLine(string.Join(
            "  ",
            widths.Select(width => new string('-', width))));

        void WriteRow(DisplayRow row)
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            try
            {
                ConsoleColor? color = GetLifetimeColor(
                    row.Asset.PurchaseDate.Date.AddYears(3),
                    today);

                if (color.HasValue)
                {
                    Console.ForegroundColor = color.Value;
                }

                Console.WriteLine(FormatLine(row.Values));
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }

        if (groupByCategory)
        {
            var categories = rows
                .GroupBy(row => row.Asset switch
                {
                    ComputerAsset => "Computers",
                    MobileAsset => "Mobile Devices",
                    _ => "Other Assets"
                })
                .OrderBy(group => group.Key);

            foreach (var category in categories)
            {
                Console.WriteLine();
                Console.WriteLine(category.Key);

                foreach (DisplayRow row in category
                    .OrderBy(row => row.Asset.PurchaseDate)
                    .ThenBy(row => row.Asset.Id))
                {
                    WriteRow(row);
                }
            }
        }
        else
        {
            foreach (DisplayRow row in rows)
            {
                WriteRow(row);
            }
        }

        // Shows the explanations once, after all rows.
        Console.WriteLine();
        Console.WriteLine(
            "RED: less than 3 months remaining or expired.");
        Console.WriteLine(
            "YELLOW: 3 to less than 6 months remaining.");
        Console.WriteLine(
            "NORMAL: 6 months or more remaining.");

        Console.WriteLine();
        Console.WriteLine(
            "Local prices use fixed educational example exchange rates.");

        if (rows.Any(row => row.Values[7] == "N/A"))
        {
            Console.WriteLine(
                "N/A: office could not be matched uniquely, " +
                "or its currency is unsupported.");
        }
    }

    private DisplayRow CreateRow(
        Asset asset,
        IReadOnlyList<Office> offices)
    {
        Office? office = _officeResolver.Resolve(
            asset.OfficeLocation, offices);

        string currency = "N/A";
        string localPrice = "N/A";

        if (office is not null)
        {
            currency = office.CurrencyCode.Trim().ToUpperInvariant();

            if (currency is "USD" or "SEK" or "EUR" or "TRY")
            {
                decimal amount = _currencyConverter.ConvertFromUsd(
                    asset.PurchasePriceUsd,
                    currency);

                localPrice = FormatPrice(amount);
            }
        }

        string type = asset switch
        {
            ComputerAsset => "Computer",
            MobileAsset => "Mobile",
            _ => asset.AssetType
        };

        return new DisplayRow(
            asset,
            [
                type,
                asset.Brand,
                asset.ModelName,
                GetOfficeLabel(asset.OfficeLocation),
                asset.PurchaseDate.ToString(
                    "yyyy-MM-dd", CultureInfo.InvariantCulture),
                FormatPrice(asset.PurchasePriceUsd),
                currency,
                localPrice
            ]);
    }

    // The Currency column identifies the unit, so no symbol is needed.
    private static string FormatPrice(decimal amount)
    {
        return amount.ToString("0.00", CultureInfo.InvariantCulture);
    }

    private static string GetOfficeLabel(string officeLocation)
    {
        string label = officeLocation.Trim();
        const string suffix = " Office";

        return label.EndsWith(
            suffix, StringComparison.OrdinalIgnoreCase)
                ? label[..^suffix.Length].TrimEnd()
                : label;
    }

    private static ConsoleColor? GetLifetimeColor(
        DateTime endOfLife,
        DateTime today)
    {
        if (endOfLife < today.AddMonths(3))
        {
            return ConsoleColor.Red;
        }

        if (endOfLife < today.AddMonths(6))
        {
            return ConsoleColor.Yellow;
        }

        return null;
    }

    // Holds the asset and its prepared display values.
    private sealed record DisplayRow(Asset Asset, string[] Values);
}