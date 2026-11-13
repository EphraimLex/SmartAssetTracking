/*
    Smart Asset Tracking — Level 5.
    Reuses Levels 1–4 and adds simulated login, employees,
    asset assignment, maintenance, and dashboard statistics.
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
using AssetTracking.Level4.ConsoleUI.Actions;
using AssetTracking.Level4.Exporting;
using AssetTracking.Level4.Services;
using AssetTracking.Level5.ConsoleUI;
using AssetTracking.Level5.ConsoleUI.Actions;
using AssetTracking.Level5.Data;
using AssetTracking.Level5.Models;
using AssetTracking.Level5.Services;
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
    // Reuses Level 3's context for existing assets and offices.
    var options = new DbContextOptionsBuilder<OfficeAssetContext>()
        .UseSqlServer(
            connectionString,
            sqlOptions => sqlOptions.MigrationsHistoryTable(
                "__EFMigrationsHistory_Level3"))
        .Options;

    using OfficeAssetContext context = new(options);

    // Both repositories share the asset and office context.
    IAssetRepository assetRepository = new AssetRepository(context);
    IOfficeRepository officeRepository = new OfficeRepository(context);

    // Collects console input.
    ConsoleInput consoleInput = new();

    // Stores Level 5 data in the same database using separate
    // migration history for employee and maintenance tables.
    var enterpriseOptions =
        new DbContextOptionsBuilder<EnterpriseContext>()
            .UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.MigrationsHistoryTable(
                    "__EFMigrationsHistory_Level5"))
            .Options;

    using EnterpriseContext enterpriseContext = new(enterpriseOptions);

    // Configures employee, assignment, and maintenance services.
    EmployeeService employeeService = new(
        enterpriseContext,
        assetRepository);

    MaintenanceService maintenanceService = new(
        enterpriseContext,
        assetRepository);

    // Creates a demo administrator only when no employees exist.
    // Login is a username-only educational simulation.
    LoginService loginService = new(employeeService);
    loginService.EnsureDemoAdmin();

    LoginScreen loginScreen = new(consoleInput, loginService);
    Employee currentUser = loginScreen.Show();

    // Matches alternative office labels to registered offices.
    OfficeResolver officeResolver = new();

    // Reuses office matching when calculating dashboard totals.
    DashboardService dashboardService = new(
        assetRepository,
        employeeService,
        officeRepository,
        officeResolver);

    // Converts USD using fixed educational exchange rates.
    ICurrencyConverter currencyConverter = new FixedCurrencyConverter();

    // Reuses the asset table with office and currency information.
    IAssetDisplay assetDisplay = new OfficeAssetDisplay(
        officeRepository,
        officeResolver,
        currencyConverter);

    // Displays registered offices and supports office selection.
    OfficeDisplay officeDisplay = new();

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

    // Generates asset counts and purchase-price totals by office.
    IOfficeAssetReportService officeReportService =
        new OfficeAssetReportService(
            assetRepository,
            officeRepository,
            officeResolver,
            currencyConverter);

    OfficeAssetReportDisplay officeReportDisplay = new();

    // Reuses Level 4 search, filtering, and export services.
    AssetSearchService assetSearch = new();
    AssetReportExporter assetExporter = new();

    // Registers the capabilities inherited from Levels 1–4.
    IMenuAction[] actions =
    [
        new AddAssetAction(assetInput, assetRepository),

        new DisplayAssetsAction(assetRepository, assetDisplay),

        new AssetSelectionListAction(
            new UpdateAssetAction(assetInput, assetRepository),
            assetRepository),

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

        new DisplayMostExpensiveAssetsAction(
            assetRepository,
            assetDisplay),

        new SearchAssetsAction(
            consoleInput,
            assetRepository,
            assetDisplay,
            assetSearch,
            assetExporter)
    ];

    // Registers Level 5's employee and maintenance actions.
    IMenuAction employeeAction = new ManageEmployeesAction(
        consoleInput,
        employeeService,
        currentUser);

    IMenuAction maintenanceAction = new ManageMaintenanceAction(
        consoleInput,
        maintenanceService,
        assetRepository,
        currentUser);

    IMenuAction dashboardAction =
        new DisplayDashboardAction(dashboardService);

    // Employees can view their own assignments and maintenance.
    // Admin and Manager retain the management features.
    // Employee registration is restricted to Admin inside its action.
    IMenuAction[] availableActions =
        currentUser.Role == UserRole.Employee
            ? [employeeAction, maintenanceAction]
            : [
                .. actions,
                employeeAction,
                maintenanceAction,
                dashboardAction
              ];

    ConsoleMenu menu = new(availableActions);

    menu.Run();

    loginService.Logout();
}
catch (DbUpdateException)
{
    Console.WriteLine();
    Console.WriteLine("The changes could not be saved.");
    Console.WriteLine(
        "Check the database connection and the entered values.");
}
catch (SqlException exception)
{
    Console.WriteLine();
    Console.WriteLine($"SQL Server error {exception.Number}:");
    Console.WriteLine(exception.Message);
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