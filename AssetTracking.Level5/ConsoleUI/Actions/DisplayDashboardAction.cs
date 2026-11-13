using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level5.Services;

namespace AssetTracking.Level5.ConsoleUI.Actions;

// Displays statistics prepared by the dashboard service.
public class DisplayDashboardAction : IMenuAction
{
    private readonly DashboardService _dashboard;

    public DisplayDashboardAction(DashboardService dashboard)
    {
        _dashboard = dashboard;
    }

    public string Key => "16";

    public string Description => "Management dashboard";

    public void Execute()
    {
        var summary = _dashboard.GetSummary();

        Console.WriteLine();
        Console.WriteLine("========== MANAGEMENT DASHBOARD ==========");
        Console.WriteLine($"Total assets: {summary.TotalAssets}");
        Console.WriteLine(
            $"Total purchase value (USD): {summary.TotalValueUsd:F2}");
        Console.WriteLine($"Expired assets: {summary.ExpiredAssets}");
        Console.WriteLine(
            $"Expiring within six months: {summary.ExpiringAssets}");
        Console.WriteLine($"Unassigned assets: {summary.UnassignedAssets}");
        Console.WriteLine($"Most used asset type: {summary.MostUsedTypes}");

        Console.WriteLine();
        Console.WriteLine("PURCHASE VALUE BY OFFICE (USD)");

        foreach (var office in summary.OfficeValues)
        {
            Console.WriteLine($"{office.Key}: {office.Value:F2}");
        }

        Console.WriteLine();
        Console.WriteLine("ASSETS PER REGISTERED EMPLOYEE");

        foreach (var employee in summary.EmployeeAssetCounts)
        {
            Console.WriteLine($"{employee.Key}: {employee.Value}");
        }
    }
}