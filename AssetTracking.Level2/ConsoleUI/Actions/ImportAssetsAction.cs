using System.Text.Json;
using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level1.Models;
using AssetTracking.Level1.Repositories;
using AssetTracking.Level2.Importing;

namespace AssetTracking.Level2.ConsoleUI.Actions;

// Reads assets from a JSON file and saves them together.
public class ImportAssetsAction : IMenuAction
{
    private readonly ConsoleInput _input;
    private readonly AssetFileReader _reader;
    private readonly IAssetRepository _repository;

    public ImportAssetsAction(
        ConsoleInput input,
        AssetFileReader reader,
        IAssetRepository repository)
    {
        _input = input;
        _reader = reader;
        _repository = repository;
    }

    public string Key => "5";

    public string Description => "Import assets from JSON";

    public void Execute()
    {
        Console.WriteLine();

        // Allows paths pasted using Windows "Copy as path".
        string filePath = _input.ReadRequiredText(
            "JSON file path: ").Trim().Trim('"');

        IReadOnlyList<Asset> assets;

        try
        {
            // Read and validate the whole file before saving.
            assets = _reader.Read(filePath);
        }
        catch (JsonException)
        {
            Console.WriteLine(
                "Import cancelled: the file contains invalid JSON.");
            return;
        }
        catch (FormatException exception)
        {
            Console.WriteLine($"Import cancelled: {exception.Message}");
            return;
        }
        catch (IOException exception)
        {
            Console.WriteLine($"Could not read the file: {exception.Message}");
            return;
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("You do not have permission to read this file.");
            return;
        }
        catch (ArgumentException)
        {
            Console.WriteLine("The file path is invalid.");
            return;
        }
        catch (NotSupportedException)
        {
            Console.WriteLine("The file path format is not supported.");
            return;
        }

        // Database errors are handled by Program.cs.
        _repository.AddRange(assets);

        Console.WriteLine(
            $"{assets.Count} assets imported successfully.");
    }
}