/*
    Smart Asset Tracking — Level 2.
    Configures the database connection and connects the application components.
    Supports CRUD, JSON import, and displaying assets by category.
*/

using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level1.Data;
using AssetTracking.Level1.Repositories;
using AssetTracking.Level2.ConsoleUI.Actions;
using AssetTracking.Level2.Importing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

// Uses the same database as Level 1.
string connectionString =
    @"Server=localhost\SQLEXPRESS;" +
    "Database=asset_tracking;" +
    "Integrated Security=True;" +
    "TrustServerCertificate=True;";

try
{
    // Configures AssetContext to use SQL Server.
    var options = new DbContextOptionsBuilder<AssetContext>()
        .UseSqlServer(connectionString)
        .Options;

    using AssetContext context = new(options);

    // Handles database operations.
    IAssetRepository repository = new AssetRepository(context);

    // Collects input and displays assets.
    ConsoleInput consoleInput = new();
    AssetInput assetInput = new(consoleInput);
    AssetDisplay assetDisplay = new();

    // Reads and validates assets from JSON files.
    AssetFileReader assetFileReader = new();

    // Registers the available menu actions.
    IMenuAction[] actions =
    [
        new AddAssetAction(assetInput, repository),
        new DisplayAssetsAction(repository, assetDisplay),
        new UpdateAssetAction(assetInput, repository),
        new DeleteAssetAction(assetInput, repository),
        new ImportAssetsAction(
            consoleInput,
            assetFileReader,
            repository),
        new DisplayAssetsByCategoryAction(repository, assetDisplay)
    ];

    ConsoleMenu menu = new(actions);

    menu.Run();
}
catch (DbUpdateException)
{
    Console.WriteLine();
    Console.WriteLine("The asset changes could not be saved.");
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