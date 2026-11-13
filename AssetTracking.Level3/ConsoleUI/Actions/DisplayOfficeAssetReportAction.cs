using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level3.Reporting;

namespace AssetTracking.Level3.ConsoleUI.Actions;

// Generates the office asset report and passes it to the display.
public class DisplayOfficeAssetReportAction : IMenuAction
{
    private readonly IOfficeAssetReportService _reportService;
    private readonly OfficeAssetReportDisplay _display;

    public DisplayOfficeAssetReportAction(
        IOfficeAssetReportService reportService,
        OfficeAssetReportDisplay display)
    {
        _reportService = reportService;
        _display = display;
    }

    public string Key => "10";

    public string Description => "Display asset totals by office";

    public void Execute()
    {
        OfficeAssetReportResult report = _reportService.Generate();

        _display.Display(report);
    }
}