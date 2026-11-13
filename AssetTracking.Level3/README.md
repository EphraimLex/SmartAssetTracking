# Smart Asset Tracking — Level 3

Level 3 extends Level 2 with office management, local-currency
prices, and asset reports.

## Features

- Create, display, update, and delete assets.
- Import assets from JSON.
- Display assets by category and purchase date.
- Highlight assets according to their remaining lifetime.
- Add and display company offices.
- Import offices from JSON with duplicate checking.
- Select registered offices when creating or updating assets.
- Display purchase prices in USD and local currency.
- Report asset counts and purchase-price totals per office.
- Display expiring and expired assets.
- Display the five most expensive assets.

## Menu

| Option | Action |
| --- | --- |
| 1 | Add asset |
| 2 | Display all assets |
| 3 | Update asset |
| 4 | Delete asset |
| 5 | Import assets from JSON |
| 6 | Display assets by category |
| 7 | Add office |
| 8 | Display offices |
| 9 | Import offices from JSON |
| 10 | Display asset totals by office |
| 11 | Display expiring and expired assets |
| 12 | Display most expensive assets |
| 0 | Exit |

Update and delete display an asset ID list before requesting
a selection. Enter 0 at the asset ID prompt to cancel.

Registered-office selection also accepts 0 to cancel.

## Technologies

- C# and .NET 10
- .NET Console Application
- Entity Framework Core
- SQL Server
- LINQ
- JSON

## Project Structure

Level 3 references Level 2 and reuses components from the
earlier levels.

### Models

- Office: stores the office name, country, and currency code.

### Data

- OfficeAssetContext: extends AssetContext with offices.
- OfficeAssetContextFactory: creates the context for migrations.

### Repositories

- IOfficeRepository: defines office database operations.
- OfficeRepository: implements those operations with EF Core.

### Importing

- OfficeFileReader: reads and validates office JSON files.

### Services

- ICurrencyConverter: defines conversion from USD.
- FixedCurrencyConverter: uses illustrative fixed exchange rates.
- OfficeResolver: matches an asset's office text to a registered office.

### Reporting

- OfficeAssetReport: holds totals for one office.
- OfficeAssetReportResult: holds office summaries and unmatched assets.
- IOfficeAssetReportService: defines report generation.
- OfficeAssetReportService: calculates office counts and totals.

### ConsoleUI

- OfficeInput: collects office details.
- OfficeDisplay: displays registered offices.
- RegisteredOfficeLocationInput: selects an office by ID.
- OfficeAssetDisplay: provides the Level 3 asset table.
- OfficeAssetReportDisplay: displays office totals and unmatched assets.
- Actions: contains the additional menu actions.

## Design

Each level's Program.cs selects its available menu actions
and supplies the implementations they use.

ConsoleMenu displays and executes the registered actions.

Asset display actions depend on IAssetDisplay:

- Earlier levels use AssetDisplay.
- Level 3 uses OfficeAssetDisplay.

AssetInput depends on IOfficeLocationInput:

- Earlier levels use TextOfficeLocationInput.
- Level 3 uses RegisteredOfficeLocationInput.

AssetSelectionListAction wraps the existing update and delete
actions to show selectable asset IDs.

Database access, input, display, conversion, and reporting
calculations have separate responsibilities.

## Database and Migrations

Level 3 uses the existing database:

- Server: localhost\SQLEXPRESS
- Database: asset_tracking
- Authentication: Windows Authentication

Connection string:

    Server=localhost\SQLEXPRESS;Database=asset_tracking;Integrated Security=True;TrustServerCertificate=True;

The contexts manage separate parts of the database:

| Context | Managed table | Migration history |
| --- | --- | --- |
| AssetContext | Assets | __EFMigrationsHistory |
| OfficeAssetContext | Offices | __EFMigrationsHistory_Level3 |

OfficeAssetContext can read and write assets, but excludes
the Assets table from its migrations.

The AddOffices migration creates the Offices table.
It does not recreate the Assets table.

For a fresh database, apply Level 1 migrations before Level 3:

```powershell
Update-Database -Context AssetContext -Project AssetTracking.Level1 -StartupProject AssetTracking.Level1
Update-Database -Context OfficeAssetContext -Project AssetTracking.Level3 -StartupProject AssetTracking.Level3
```

The reporting classes are not database entities and require
no migrations.

## Office Data

Each office contains:

| Property | Description |
| --- | --- |
| Id | Database-generated identifier |
| OfficeName | Required text, maximum 100 characters |
| Country | Required text, maximum 100 characters |
| CurrencyCode | Supported code: USD, SEK, EUR, or TRY |

Office input normalizes currency codes to uppercase.

## Office JSON Import

Save office entries in a JSON array, even for a single office.

Example offices.json:

```json
[
  {
    "OfficeName": "Germany Office",
    "Country": "Germany",
    "CurrencyCode": "EUR"
  }
]
```

Office property names are matched without regard to letter case.
For example, OfficeName and officeName are both accepted,
but cannot both appear in the same entry.

The reader validates every entry before saving:

- The root must be a non-empty array.
- Each entry must be an object.
- Required fields must contain non-empty text.
- OfficeName and Country must not exceed 100 characters.
- CurrencyCode must be USD, SEK, EUR, or TRY.

Select option 9 and enter an ordinary Windows file path:

```text
JSON file path: C:\Users\Ephraim\Documents\offices.json
```

Do not double the backslashes or add a C# @ prefix.
Paths pasted with surrounding quotation marks are also accepted.

### Duplicate Handling

The import identifies offices by office name and country,
ignoring case and surrounding whitespace.

- Matching entries with the same currency are skipped.
- Duplicate entries within the file are also skipped.
- Conflicting currencies cancel the import before saving.
- New offices are saved together using one SaveChanges call.

This duplicate check belongs to the import workflow.
Manual office creation does not currently enforce uniqueness,
and no database uniqueness constraint has been added.

## Asset JSON Import

Option 5 reuses Level 2's asset importer.

See the Level 2 README for the JSON structure and validation rules.

Asset import uses the OfficeLocation text supplied in the file.
It does not use the registered-office selector or automatically
validate that the office exists.

## Office Matching

Assets currently store OfficeLocation as text.
There is no OfficeId foreign key connecting assets to offices.

OfficeResolver matches names without regard to letter case,
surrounding whitespace, or a trailing " Office" suffix.

Examples:

| Asset location | Registered office | Result |
| --- | --- | --- |
| Sweden | Sweden Office | Match |
| Turkey Office | Turkey Office | Match |
| Germany | Germany Office | Match |
| New York | USA Office | No match |

A location must match exactly one registered office.
Missing or ambiguous matches remain unresolved.

Adding offices does not automatically change existing asset
locations. Use Update asset → Office location to select a
registered office for an existing asset.

The display removes a trailing " Office" suffix from labels.
Stored names remain unchanged.

## Currency Conversion

Purchase prices remain stored in USD.

Local prices are calculated for display and reporting using
these fixed educational example rates:

| Currency | Local currency units per 1 USD |
| --- | --- |
| USD | 1.00 |
| SEK | 10.00 |
| EUR | 0.90 |
| TRY | 35.00 |

These are illustrative rates, not current market rates.

Converted amounts are rounded to two decimal places using
MidpointRounding.AwayFromZero.

The asset table shows N/A when the office cannot be matched
uniquely or its currency is unsupported.

## Asset Display

The Level 3 table contains:

- Type
- Brand
- Model
- Office
- Purchase Date
- Price in USD
- Currency
- Local Price

Prices are displayed numerically with two decimal places.
The Currency column identifies the local currency.

The general list is ordered by ID.
The category view groups computers and mobile devices, then
sorts each group by purchase date and ID.

## Lifetime Colours

Expected asset lifetime is three years from purchase.

| Remaining lifetime | Colour |
| --- | --- |
| Expired or less than 3 months | Red |
| 3 months to less than 6 months | Yellow |
| 6 months or more | Normal |

The legend appears once below the complete table.

These rules concern asset lifetime, not warranty expiration.

## Reports

### Asset Totals by Office — Option 10

Displays:

- Every registered office, including offices with no assets.
- Asset count per office.
- Total USD purchase price per office.
- Local-currency purchase-price total per office.
- Overall asset count and USD total.
- A separate list of unmatched assets.

Unmatched assets are included in overall totals, but not in
any office total.

Local totals sum individually rounded asset conversions,
matching the amounts shown in the asset table.

Different local currencies are not added together.
Values represent purchase prices, not depreciated values.

### Expiring and Expired Assets — Option 11

Displays assets with less than six months remaining in their
three-year lifetime, including expired assets.

Assets with exactly six months or more remaining are excluded.

### Most Expensive Assets — Option 12

Displays up to five assets ranked by descending USD purchase
price. Equal prices are ordered by asset ID.

Ranking uses USD so prices are compared in the same currency.

## Running the Application

1. Ensure SQL Server is running.
2. Check the connection string.
3. Ensure the required migrations have been applied.
4. Set AssetTracking.Level3 as the startup project.
5. Run the application.
6. Add or import offices.
7. Use the menu to manage assets and generate reports.

Widen the console window to accommodate the asset table.

## Verified Functionality

Manually tested:

- Office JSON import and repeated-import duplicate skipping.
- Registered-office selection.
- Level 3 asset table and category view.
- Short office display labels.
- Local-currency price display.
- ID lists and cancellation for update and delete.
- Asset counts and totals by office.
- Expiring and expired asset report.
- Most expensive assets ordered by USD purchase price.

## Current Limitations

- Office associations use text matching rather than foreign keys.
- Asset JSON import can contain unregistered office locations.
- Manual office creation can create duplicate offices.
- Exchange rates are fixed examples.
- Database errors handled by Program.cs end the current run.

## Next Level

Level 4 introduces advanced search, filtering, and export:

- Search by brand, model, office, and purchase year.
- Filter expired assets and asset categories.
- Export reports to TXT, CSV, and JSON.