using System.Globalization;
using AssetTracking.Level3.Reporting;

namespace AssetTracking.Level3.ConsoleUI;

// Displays office totals and identifies assets without an office match.
public class OfficeAssetReportDisplay
{
    public void Display(OfficeAssetReportResult result)
    {
        Console.WriteLine();
        Console.WriteLine("ASSET TOTALS BY OFFICE");
        Console.WriteLine("=====================");
        Console.WriteLine();

        if (result.Offices.Count == 0)
        {
            Console.WriteLine("No registered offices found.");
        }
        else
        {
            DisplayOfficeTotals(result.Offices);
        }

        // Includes unmatched assets in the overall count and USD total.
        int totalCount =
            result.Offices.Sum(office => office.AssetCount) +
            result.UnmatchedAssets.Count;

        decimal unmatchedUsd = result.UnmatchedAssets.Sum(
            asset => asset.PurchasePriceUsd);

        decimal totalUsd =
            result.Offices.Sum(office => office.TotalValueUsd) +
            unmatchedUsd;

        Console.WriteLine();
        Console.WriteLine($"Total assets: {totalCount}");
        Console.WriteLine(
            $"Total purchase value (USD): {FormatPrice(totalUsd)}");

        if (result.UnmatchedAssets.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("ASSETS WITHOUT A UNIQUE OFFICE MATCH");
            Console.WriteLine("------------------------------------");

            foreach (var asset in result.UnmatchedAssets)
            {
                Console.WriteLine(
                    $"ID {asset.Id}: {asset.Brand} {asset.ModelName} " +
                    $"| Office: {asset.OfficeLocation} " +
                    $"| USD: {FormatPrice(asset.PurchasePriceUsd)}");
            }

            Console.WriteLine();
            Console.WriteLine(
                $"Unmatched assets: {result.UnmatchedAssets.Count}");

            Console.WriteLine(
                $"Unmatched purchase value (USD): " +
                $"{FormatPrice(unmatchedUsd)}");

            Console.WriteLine(
                "These assets are included in the overall totals, " +
                "but not in any office total.");
        }

        Console.WriteLine();
        Console.WriteLine(
            "Values are purchase-price totals, not depreciated values.");

        Console.WriteLine(
            "Local totals use fixed educational example exchange rates.");

        Console.WriteLine(
            "Local totals in different currencies are not added together.");

        if (result.Offices.Any(office => office.TotalValueLocal is null))
        {
            Console.WriteLine(
                "N/A: local total is unavailable for this currency.");
        }
    }

    private static void DisplayOfficeTotals(
        IReadOnlyList<OfficeAssetReport> offices)
    {
        string[] headings =
        [
            "Office",
            "Country",
            "Assets",
            "Total USD",
            "Currency",
            "Local Total"
        ];

        var rows = offices.Select(office => new[]
        {
            GetOfficeLabel(office.OfficeName),
            office.Country,
            office.AssetCount.ToString(CultureInfo.InvariantCulture),
            FormatPrice(office.TotalValueUsd),
            office.CurrencyCode,
            office.TotalValueLocal.HasValue
                ? FormatPrice(office.TotalValueLocal.Value)
                : "N/A"
        }).ToList();

        int[] widths = headings
            .Select((heading, index) => Math.Max(
                heading.Length,
                rows.Max(row => row[index].Length)))
            .ToArray();

        string FormatLine(string[] values)
        {
            return string.Join(
                "  ",
                values.Select((value, index) =>
                    index is 2 or 3 or 5
                        ? value.PadLeft(widths[index])
                        : value.PadRight(widths[index])));
        }

        Console.WriteLine(FormatLine(headings));

        Console.WriteLine(string.Join(
            "  ",
            widths.Select(width => new string('-', width))));

        foreach (string[] row in rows)
        {
            Console.WriteLine(FormatLine(row));
        }
    }

    private static string FormatPrice(decimal amount)
    {
        return amount.ToString("0.00", CultureInfo.InvariantCulture);
    }

    private static string GetOfficeLabel(string officeName)
    {
        string label = officeName.Trim();
        const string suffix = " Office";

        return label.EndsWith(
            suffix, StringComparison.OrdinalIgnoreCase)
                ? label[..^suffix.Length].TrimEnd()
                : label;
    }
}