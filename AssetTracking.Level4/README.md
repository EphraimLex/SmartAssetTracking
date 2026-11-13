# Smart Asset Tracking — Level 4

Level 4 extends Level 3 with asset search, filtering, and report export.

## Features

Retains the functionality from Levels 1–3:

- Asset registration, display, update, and deletion.
- Asset import from JSON.
- Sorting and lifetime highlighting.
- Office registration, display, and JSON import.
- Local currency conversion using fixed educational exchange rates.
- Asset totals by office.
- Reports for expiring assets and the most expensive assets.

Adds menu option **13 — Search, filter, and export assets**.

### Search and Filtering

Search by:

- Brand.
- Model.
- Office.
- Purchase year.

Additional filters:

- All categories, computers, or mobile devices.
- Expired assets only.

Criteria can be combined. An asset must satisfy all selected criteria.

Press Enter to skip an optional search field. Category requires a
selection: 0 for all, 1 for computers, or 2 for mobiles.

Answering `n` to “Only expired assets?” includes both expired and
unexpired assets while preserving the other selected filters.

Asset expiration is based on a three-year lifetime from the
purchase date, independently of the warranty expiration date.

### Export

Export matching assets to:

- **TXT** — readable report with asset count and total USD price.
- **CSV** — column-based data for spreadsheets.
- **JSON** — structured data.

Exports preserve stored asset values. For example, a Desktop may
appear as Computer in the console table but remains Desktop in
the exported data.

## Project Structure

- `ConsoleUI/Actions/SearchAssetsAction.cs`
  Collects criteria, displays matching assets, and handles export choices.
- `Services/AssetSearchService.cs`
  Applies search and filtering criteria.
- `Exporting/AssetReportExporter.cs`
  Generates TXT, CSV, and JSON reports.
- `Program.cs`
  Configures dependencies and registers menu actions.

## Dependencies

Level 4 references Level 3 and reuses functionality from the
earlier projects.

Technologies:

- C# / .NET 10.
- Entity Framework Core.
- SQL Server Express.
- LINQ.

## Database

- Server: `localhost\SQLEXPRESS`
- Database: `asset_tracking`
- Authentication: Windows Authentication
- Context: `OfficeAssetContext`
- Migration history: `__EFMigrationsHistory_Level3`

Level 4 introduces no database schema changes and therefore
requires no additional migration. The Level 3 database setup
must already be applied.

## Running the Application

1. Ensure SQL Server is running.
2. Set `AssetTracking.Level4` as the startup project.
3. Build and run the application.
4. Select option `13` to search, filter, and export.
5. Enter the desired criteria.
6. Choose an export format or enter `0` to return.
7. For an export, provide a new output file path.

## Manual Validation Completed

- Brand search returned the ASUS ExpertCenter D7 record.
- Purchase-year filtering returned the matching 2026 asset.
- Expired-only filtering returned the Dell asset purchased in 2023.
- Combined ASUS and 2023 criteria returned no matching assets.
- CSV, JSON, and TXT exports were checked against the ASUS record.

## Design

Console interaction, search logic, and export formatting are
separated into dedicated classes. Existing repository and display
abstractions are reused to avoid duplicating earlier functionality.