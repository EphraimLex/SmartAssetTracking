using System.Globalization;
using System.Text;
using System.Text.Json;
using AssetTracking.Level1.Models;

namespace AssetTracking.Level4.Exporting;

// Exports the selected assets without changing the database.
public class AssetReportExporter
{
    public void Export(
        IReadOnlyList<Asset> assets,
        string filePath,
        string format)
    {
        string content = format switch
        {
            "1" => CreateText(assets),
            "2" => CreateCsv(assets),
            "3" => CreateJson(assets),
            _ => throw new ArgumentException("Unsupported export format.")
        };

        // Prevents accidental overwriting of an existing file.
        using FileStream stream = new(
            filePath,
            FileMode.CreateNew,
            FileAccess.Write);

        using StreamWriter writer = new(
            stream,
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

        writer.Write(content);
    }

    private static string CreateJson(IReadOnlyList<Asset> assets)
    {
        var records = assets.Select(asset => new
        {
            asset.Id,
            asset.AssetType,
            asset.Brand,
            asset.ModelName,
            PurchaseDate = Date(asset.PurchaseDate),
            asset.PurchasePriceUsd,
            asset.OfficeLocation,
            asset.SerialNumber,
            asset.EmployeeUsername,
            WarrantyExpirationDate = Date(asset.WarrantyExpirationDate)
        });

        return JsonSerializer.Serialize(
            records,
            new JsonSerializerOptions { WriteIndented = true });
    }

    private static string CreateCsv(IReadOnlyList<Asset> assets)
    {
        StringBuilder text = new();

        text.AppendLine(
            "Id,AssetType,Brand,ModelName,PurchaseDate," +
            "PurchasePriceUsd,OfficeLocation,SerialNumber," +
            "EmployeeUsername,WarrantyExpirationDate");

        foreach (Asset asset in assets)
        {
            text.AppendLine(string.Join(
                ",",
                Values(asset).Select(CsvCell)));
        }

        return text.ToString();
    }

    private static string CreateText(IReadOnlyList<Asset> assets)
    {
        StringBuilder text = new();

        text.AppendLine("ASSET REPORT");
        text.AppendLine($"Assets: {assets.Count}");
        text.AppendLine(
            "Total purchase price (USD): " +
            assets.Sum(asset => asset.PurchasePriceUsd)
                .ToString("0.00", CultureInfo.InvariantCulture));
        text.AppendLine();

        string[] labels =
        [
            "ID", "Type", "Brand", "Model", "Purchase date",
            "Price USD", "Office", "Serial number", "Employee",
            "Warranty expiration"
        ];

        foreach (Asset asset in assets)
        {
            string[] values = Values(asset);

            for (int index = 0; index < labels.Length; index++)
            {
                text.AppendLine($"{labels[index]}: {values[index]}");
            }

            text.AppendLine();
        }

        return text.ToString();
    }

    private static string[] Values(Asset asset)
    {
        return
        [
            asset.Id.ToString(CultureInfo.InvariantCulture),
            asset.AssetType,
            asset.Brand,
            asset.ModelName,
            Date(asset.PurchaseDate),
            asset.PurchasePriceUsd.ToString(
                "0.00", CultureInfo.InvariantCulture),
            asset.OfficeLocation,
            asset.SerialNumber,
            asset.EmployeeUsername ?? "",
            Date(asset.WarrantyExpirationDate)
        ];
    }

    private static string Date(DateTime value)
    {
        return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static string CsvCell(string value)
    {
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}