/*
    Smart Asset Tracking — Level 3.
    Configures the database connection and application components.
    Supports asset management, office management, JSON imports,
    local currency prices, and asset totals by office.
*/

using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level1.Repositories;
using AssetTracking.Level2.ConsoleUI.Actions;
using AssetTracking.Level2.Importing;
using AssetTracking.Level3.ConsoleUI;
using AssetTracking.Level3.ConsoleUI.Actions;
using AssetTracking.Level3.Data;
using AssetTracking.Level3.Importing;
using AssetTracking.Level3.Reporting;
using AssetTracking.Level3.Repositories;
using AssetTracking.Level3.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

// Supports Unicode characters in console output.
Console.OutputEncoding = System.Text.Encoding.UTF8;

string connectionString =
    @"Server=localhost\SQLEXPRESS;" +
    "Database=asset_tracking;" +
    "Integrated Security=True;" +
    "TrustServerCertificate=True;";

try
{
    // Uses the same configuration as the Level 3 design-time factory.
    var options = new DbContextOptionsBuilder<OfficeAssetContext>()
        .UseSqlServer(
            connectionString,
            sqlOptions =>
            {
                // Tracks Level 3 migrations separately from Level 1.
                sqlOptions.MigrationsHistoryTable(
                    "__EFMigrationsHistory_Level3");
            })
        .Options;

    using OfficeAssetContext context = new(options);

    // Both repositories use the same context and database.
    IAssetRepository assetRepository = new AssetRepository(context);
    IOfficeRepository officeRepository = new OfficeRepository(context);

    // Collects console input.
    ConsoleInput consoleInput = new();

    // Matches office names and converts USD to local currency.
    OfficeResolver officeResolver = new();
    ICurrencyConverter currencyConverter = new FixedCurrencyConverter();

    // Supplies Level 3's asset table layout.
    IAssetDisplay assetDisplay = new OfficeAssetDisplay(
        officeRepository,
        officeResolver,
        currencyConverter);

    // Displays registered offices and supports office selection.
    OfficeDisplay officeDisplay = new();

    // Uses registered offices instead of free-text office input.
    IOfficeLocationInput officeLocationInput =
        new RegisteredOfficeLocationInput(
            consoleInput,
            officeRepository,
            officeDisplay);

    // Collects asset and office details.
    AssetInput assetInput = new(
        consoleInput,
        officeLocationInput);

    OfficeInput officeInput = new(consoleInput);

    // Reads and validates JSON files before importing.
    AssetFileReader assetFileReader = new();
    OfficeFileReader officeFileReader = new();

    // Calculates asset counts and purchase-price totals for each office.
    IOfficeAssetReportService officeReportService =
        new OfficeAssetReportService(
            assetRepository,
            officeRepository,
            officeResolver,
            currencyConverter);

    // Formats office totals and lists unmatched assets.
    OfficeAssetReportDisplay officeReportDisplay = new();

    // Registers the capabilities available in Level 3.
    IMenuAction[] actions =
    [
        new AddAssetAction(assetInput, assetRepository),

        new DisplayAssetsAction(assetRepository, assetDisplay),

        // Shows asset IDs before prompting for an asset to update.
        new AssetSelectionListAction(
            new UpdateAssetAction(assetInput, assetRepository),
            assetRepository),

        // Shows asset IDs before prompting for an asset to delete.
        new AssetSelectionListAction(
            new DeleteAssetAction(assetInput, assetRepository),
            assetRepository),

        new ImportAssetsAction(
            consoleInput,
            assetFileReader,
            assetRepository),

        new DisplayAssetsByCategoryAction(
            assetRepository,
            assetDisplay),

        new AddOfficeAction(
            officeInput,
            officeRepository),

        new DisplayOfficesAction(
            officeRepository,
            officeDisplay),

        new ImportOfficesAction(
            consoleInput,
            officeFileReader,
            officeRepository),

        new DisplayOfficeAssetReportAction(
            officeReportService,
            officeReportDisplay),

        new DisplayExpiringAssetsAction(
            assetRepository,
            assetDisplay),

        new DisplayExpiringAssetsAction(
            assetRepository,
            assetDisplay),

        new DisplayMostExpensiveAssetsAction(
            assetRepository,
            assetDisplay)
            ];

    ConsoleMenu menu = new(actions);

    menu.Run();
}
catch (DbUpdateException)
{
    Console.WriteLine();
    Console.WriteLine("The changes could not be saved.");
    Console.WriteLine(
        "Check the database connection and the entered values.");
}
catch (SqlException)
{
    Console.WriteLine();
    Console.WriteLine("A SQL Server error occurred.");
    Console.WriteLine(
        "Check that SQL Server is running and the database is accessible.");
}
catch (InvalidOperationException exception)
    when (exception.InnerException is SqlException)
{
    Console.WriteLine();
    Console.WriteLine("The database operation could not be completed.");
    Console.WriteLine(
        "Check that SQL Server is running and the database is accessible.");
}
catch (OperationCanceledException)
{
    Console.WriteLine();
    Console.WriteLine("Input ended. The application will close.");
}