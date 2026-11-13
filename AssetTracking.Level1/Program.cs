/*
    Smart Asset Tracking — Level 1.
    Configures the database connection and connects the application components.
*/

using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level1.Data;
using AssetTracking.Level1.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

string connectionString =
    @"Server=localhost\SQLEXPRESS;" +
    "Database=asset_tracking;" +
    "Integrated Security=True;" +
    "TrustServerCertificate=True;";

try
{
    var options = new DbContextOptionsBuilder<AssetContext>()
        .UseSqlServer(connectionString)
        .Options;

    using AssetContext context = new(options);

    IAssetRepository repository = new AssetRepository(context);

    ConsoleInput consoleInput = new();
    AssetInput assetInput = new(consoleInput);
    AssetDisplay assetDisplay = new();

    IMenuAction[] actions =
    [
        new AddAssetAction(assetInput, repository),
        new DisplayAssetsAction(repository, assetDisplay)
    ];

    ConsoleMenu menu = new(actions);

    menu.Run();
}
catch (DbUpdateException)
{
    Console.WriteLine();
    Console.WriteLine("The asset could not be saved.");
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