using System.Text.Json;
using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level3.Importing;
using AssetTracking.Level3.Models;
using AssetTracking.Level3.Repositories;

namespace AssetTracking.Level3.ConsoleUI.Actions;

// Imports offices after validating the file and checking duplicates.
public class ImportOfficesAction : IMenuAction
{
    private readonly ConsoleInput _input;
    private readonly OfficeFileReader _reader;
    private readonly IOfficeRepository _repository;

    public ImportOfficesAction(
        ConsoleInput input,
        OfficeFileReader reader,
        IOfficeRepository repository)
    {
        _input = input;
        _reader = reader;
        _repository = repository;
    }

    public string Key => "9";

    public string Description => "Import offices from JSON";

    public void Execute()
    {
        Console.WriteLine();

        // Supports paths pasted using Windows "Copy as path".
        string filePath = _input.ReadRequiredText(
            "JSON file path: ").Trim().Trim('"');

        IReadOnlyList<Office> offices;

        try
        {
            // Validates the entire file before any database changes.
            offices = _reader.Read(filePath);
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

        // Includes existing offices and new entries accepted from this file.
        List<Office> knownOffices = _repository.GetAll().ToList();
        List<Office> newOffices = new();

        int skippedCount = 0;

        foreach (Office office in offices)
        {
            List<Office> matches = knownOffices
                .Where(existing => IsSameOffice(existing, office))
                .ToList();

            // A matching office must use the same currency.
            if (matches.Any(existing => !string.Equals(
                existing.CurrencyCode.Trim(),
                office.CurrencyCode,
                StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine(
                    $"Import cancelled: '{office.OfficeName}' " +
                    $"in '{office.Country}' has conflicting currencies.");

                Console.WriteLine("No offices were imported.");
                return;
            }

            if (matches.Count > 0)
            {
                skippedCount++;
                continue;
            }

            newOffices.Add(office);

            // Also detects duplicate entries within the JSON file.
            knownOffices.Add(office);
        }

        if (newOffices.Count > 0)
        {
            // Database errors are handled by Program.cs.
            _repository.AddRange(newOffices);
        }

        Console.WriteLine(
            $"Offices imported: {newOffices.Count}. " +
            $"Duplicates skipped: {skippedCount}.");
    }

    private static bool IsSameOffice(Office first, Office second)
    {
        return string.Equals(
                   first.OfficeName.Trim(),
                   second.OfficeName.Trim(),
                   StringComparison.OrdinalIgnoreCase)
               && string.Equals(
                   first.Country.Trim(),
                   second.Country.Trim(),
                   StringComparison.OrdinalIgnoreCase);
    }
}