using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level1.Repositories;
using AssetTracking.Level4.Exporting;
using AssetTracking.Level4.Services;

namespace AssetTracking.Level4.ConsoleUI.Actions;

// Searches assets and optionally exports the matching records.
public class SearchAssetsAction : IMenuAction
{
    private readonly ConsoleInput _input;
    private readonly IAssetRepository _repository;
    private readonly IAssetDisplay _display;
    private readonly AssetSearchService _search;
    private readonly AssetReportExporter _exporter;

    public SearchAssetsAction(
        ConsoleInput input,
        IAssetRepository repository,
        IAssetDisplay display,
        AssetSearchService search,
        AssetReportExporter exporter)
    {
        _input = input;
        _repository = repository;
        _display = display;
        _search = search;
        _exporter = exporter;
    }

    public string Key => "13";

    public string Description => "Search, filter, and export assets";

    public void Execute()
    {
        Console.WriteLine();
        Console.WriteLine("SEARCH AND FILTER");
        Console.WriteLine("Press Enter to skip an optional criterion.");

        string? brand = _input.ReadOptionalText("Brand contains: ");
        string? model = _input.ReadOptionalText("Model contains: ");
        string? office = _input.ReadOptionalText("Office contains: ");
        int? year = ReadYear();

        string category = ReadChoice(
            "Category: 0 = all, 1 = computers, 2 = mobiles: ",
            ["0", "1", "2"]);

        bool expiredOnly = ReadChoice(
            "Only expired assets? (y/n): ",
            ["y", "n"]) == "y";

        var results = _search.Search(
            _repository.GetAll(),
            brand,
            model,
            office,
            year,
            category,
            expiredOnly);

        Console.WriteLine();
        Console.WriteLine($"Matching assets: {results.Count}");

        if (results.Count == 0)
        {
            return;
        }

        _display.DisplayAssets(results);

        string format = ReadChoice(
            "Export: 0 = return, 1 = TXT, 2 = CSV, 3 = JSON: ",
            ["0", "1", "2", "3"]);

        if (format == "0")
        {
            return;
        }

        string extension = format switch
        {
            "1" => ".txt",
            "2" => ".csv",
            _ => ".json"
        };

        string path = _input.ReadRequiredText(
            $"New output file path ({extension}): ").Trim().Trim('"');

        try
        {
            if (!string.Equals(
                Path.GetExtension(path),
                extension,
                StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(
                    $"Export cancelled: use a {extension} file extension.");
                return;
            }

            _exporter.Export(results, path, format);

            Console.WriteLine(
                $"Exported {results.Count} assets to {path}");
        }
        catch (IOException exception)
        {
            Console.WriteLine($"Export failed: {exception.Message}");
            Console.WriteLine(
                "Use an existing folder and a new filename.");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine(
                "Export failed: no permission to write to this location.");
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine($"Export failed: {exception.Message}");
        }
        catch (NotSupportedException)
        {
            Console.WriteLine("Export failed: unsupported file path.");
        }
    }

    private int? ReadYear()
    {
        while (true)
        {
            string? value = _input.ReadOptionalText("Purchase year: ");

            if (value is null)
            {
                return null;
            }

            if (int.TryParse(value, out int year) &&
                year >= 1 && year <= 9999)
            {
                return year;
            }

            Console.WriteLine("Enter a year from 1 to 9999.");
        }
    }

    private string ReadChoice(string prompt, string[] choices)
    {
        while (true)
        {
            string value = _input.ReadRequiredText(prompt)
                .ToLowerInvariant();

            if (choices.Contains(value))
            {
                return value;
            }

            Console.WriteLine(
                $"Choose one of: {string.Join(", ", choices)}.");
        }
    }
}