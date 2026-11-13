using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level1.Repositories;
using AssetTracking.Level5.Models;
using AssetTracking.Level5.Services;

namespace AssetTracking.Level5.ConsoleUI.Actions;

// Records and displays maintenance for an existing asset.
public class ManageMaintenanceAction : IMenuAction
{
    private readonly ConsoleInput _input;
    private readonly MaintenanceService _maintenance;
    private readonly IAssetRepository _assets;
    private readonly Employee _currentUser;

    public ManageMaintenanceAction(
        ConsoleInput input,
        MaintenanceService maintenance,
        IAssetRepository assets,
        Employee currentUser)
    {
        _input = input;
        _maintenance = maintenance;
        _assets = assets;
        _currentUser = currentUser;
    }

    public string Key => "15";

    public string Description => "Asset maintenance";

    public void Execute()
    {
        Console.WriteLine();
        Console.WriteLine("1. View maintenance history");
        Console.WriteLine("2. Record maintenance (Admin or Manager)");
        Console.WriteLine("0. Return");

        int choice = EnterpriseInput.ReadNumber(
            _input, "Select: ", 0, 2);

        if (choice == 0)
        {
            return;
        }

        if (choice == 2 && _currentUser.Role == UserRole.Employee)
        {
            Console.WriteLine(
                "Only Admin or Manager can record maintenance.");
            return;
        }

        int assetId = EnterpriseInput.ReadNumber(
            _input, "Asset ID: ", 1);

        var asset = _assets.GetById(assetId);

        if (asset is null)
        {
            Console.WriteLine("Asset not found.");
            return;
        }

        if (_currentUser.Role == UserRole.Employee
            && !string.Equals(
                asset.EmployeeUsername,
                _currentUser.Username,
                StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(
                "You can view maintenance only for your assigned assets.");
            return;
        }

        Console.WriteLine($"{asset.Brand} {asset.ModelName}");

        if (choice == 1)
        {
            DisplayHistory(assetId);
            return;
        }

        try
        {
            DateTime lastDate = _input.ReadDate(
                "Last maintenance (yyyy-MM-dd): ");

            DateTime nextDate = _input.ReadDate(
                "Next maintenance (yyyy-MM-dd): ");

            string notes = _input.ReadRequiredText("Notes: ");

            _maintenance.Add(assetId, lastDate, nextDate, notes);

            Console.WriteLine("Maintenance recorded successfully.");
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    private void DisplayHistory(int assetId)
    {
        var records = _maintenance.GetForAsset(assetId);

        if (records.Count == 0)
        {
            Console.WriteLine("No maintenance records found.");
            return;
        }

        foreach (var record in records)
        {
            Console.WriteLine();
            Console.WriteLine(
                $"Last maintenance: {record.LastMaintenanceDate:yyyy-MM-dd}");

            Console.WriteLine(
                $"Next maintenance: {record.NextMaintenanceDate:yyyy-MM-dd}");

            Console.WriteLine($"Notes: {record.Notes}");
        }
    }
}