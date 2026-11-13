# Smart Asset Tracking — Level 2

Level 2 extends Level 1 with full CRUD operations, JSON asset
import, and category-based asset views.

It reuses shared components from Level 1 and registers its
additional capabilities in Program.cs.

## Features

- Add new assets.
- Display all assets in an aligned table.
- Update individual asset properties.
- Delete assets.
- Import assets from JSON files.
- Display assets grouped by category and sorted by purchase date.
- Highlight assets according to their remaining lifetime.
- Display colour explanations below the complete table.
- Validate console input and imported asset data.
- Cancel asset ID selection.
- Handle database and file-reading errors.

## Menu

| Option | Action |
| --- | --- |
| 1 | Add asset |
| 2 | Display all assets |
| 3 | Update asset |
| 4 | Delete asset |
| 5 | Import assets from JSON |
| 6 | Display assets by category |
| 0 | Exit |

Enter 0 at an asset ID prompt to cancel the current action
and return to the menu.

Enter 0 at the main menu to exit the application.

Cancellation is supported at specific prompts, not at every
input field.

## Technologies

- C# and .NET 10
- .NET Console Application
- Entity Framework Core 10
- SQL Server
- LINQ
- System.Text.Json

## Asset Information

Each asset stores:

- ID
- Asset type
- Brand
- Model name
- Purchase date
- Purchase price in USD
- Office location
- Serial number
- Employee username, if assigned
- Warranty expiration date

OfficeLocation is stored as text. Level 2 does not require
a registered office.

## Asset Display

Level 2 uses the shared AssetDisplay implementation.

The table displays:

- ID
- Brand
- Model
- Purchase date
- Lifetime status

Other asset information remains stored in the database.

### Display All Assets

Option 2 displays assets ordered by ID.

### Display by Category

Option 6 groups assets into:

- Computers
- Mobile Devices
- Other Assets, if applicable

Categories are ordered alphabetically. Assets within each
category are ordered by purchase date, then by ID.

Column widths are calculated for the complete table so category
sections remain aligned.

The colour legend appears once after the complete table.

## Asset Lifetime

Assets have an expected lifetime of three years from their
purchase date.

The current implementation uses these colour rules:

| Remaining lifetime | Colour |
| --- | --- |
| Expired or less than 3 months | Red |
| 3 months to less than 6 months | Yellow |
| 6 months or more | Normal |

The most urgent condition is checked first.

These colours describe remaining asset lifetime, not warranty
coverage.

## Project Structure

Level 2 references Level 1 and reuses its existing components.

### Reused from Level 1

- Asset, ComputerAsset, and MobileAsset
- AssetContext and its database configuration
- IAssetRepository and AssetRepository
- ConsoleInput and AssetInput
- IOfficeLocationInput and TextOfficeLocationInput
- IAssetDisplay and AssetDisplay
- ConsoleMenu and IMenuAction
- Add, display, update, and delete actions
- Action cancellation exceptions and menu handling

### Added in Level 2

| File | Responsibility |
| --- | --- |
| Importing/AssetFileReader.cs | Reads and validates asset JSON files |
| ConsoleUI/Actions/ImportAssetsAction.cs | Coordinates file import and batch saving |
| ConsoleUI/Actions/DisplayAssetsByCategoryAction.cs | Requests the category view from the supplied display |
| Program.cs | Configures dependencies and registers Level 2 actions |

## Design Principles

### Separation of Responsibilities

Input collection, file reading, validation, database operations,
display formatting, and menu coordination have separate
responsibilities.

AssetFileReader reads and validates data but does not save it.

ImportAssetsAction coordinates the import and passes validated
assets to the repository.

### Open/Closed Principle

ConsoleMenu works with IMenuAction.

Additional actions can be implemented and registered in
Program.cs without changing the menu's action-selection logic.

### Display Abstraction

Display actions depend on IAssetDisplay rather than a specific
table implementation.

The interface provides:

- DisplayAssets
- DisplayByCategory

Level 2 supplies AssetDisplay. Later levels can supply their own
display without changing these actions.

### Input Abstraction

AssetInput uses IOfficeLocationInput to obtain office locations.

Level 2 retains free-text entry through TextOfficeLocationInput.
Later levels can provide registered-office selection.

### Dependency Injection

Menu actions receive dependencies through their constructors.

For example, actions receive IAssetRepository instead of
creating their own database repository.

Program.cs creates the objects and connects them.

### Code Reuse

Level 2 builds on Level 1 through a project reference.

Each level's Program.cs determines which actions are available
and which implementations they use.

## Database

Level 2 uses the same SQL Server database as Level 1:

- Server: localhost\SQLEXPRESS
- Database: asset_tracking
- Authentication: Windows Authentication

Connection string:

```text
Server=localhost\SQLEXPRESS;Database=asset_tracking;Integrated Security=True;TrustServerCertificate=True;
```

Level 2 uses AssetContext and the migrations maintained in
Level 1. It does not introduce a separate migration history.

Changes to asset records through either project affect the
same Assets table.

## Running the Application

1. Ensure SQL Server is running.
2. Check the connection string in Program.cs.
3. Ensure the required Level 1 migrations have been applied.
4. Set AssetTracking.Level2 as the startup project.
5. Run the application.
6. Select an action from the menu.

If the database needs to be initialized, apply the existing
Level 1 migrations from Package Manager Console:

```powershell
Update-Database -Context AssetContext -Project AssetTracking.Level1 -StartupProject AssetTracking.Level1
```

Do not recreate migrations that are already included.

## Creating and Updating Assets

Enter dates in yyyy-MM-dd format:

```text
2026-01-15
```

Enter USD prices using a decimal point:

```text
1250.50
```

Required text fields cannot be blank.

Warranty expiration must be on or after the purchase date.
When changing a purchase date, it cannot be after the existing
warranty expiration date.

Employee assignment is optional.

The update action lets the user select one property to change.
Computer assets also support changing between Laptop and Desktop.

## JSON Import

Use option 5, Import assets from JSON, to import assets
from a .json file.

### File Structure

The file must contain a JSON array enclosed in square brackets.
Each object inside the array represents one asset.

Even when importing only one asset, the surrounding array
is required.

### Example: One Asset

Save the following content in a file named assets.json:

```json
[
  {
    "AssetType": "Laptop",
    "Brand": "Lenovo",
    "ModelName": "ThinkPad T14",
    "PurchaseDate": "2026-01-15",
    "PurchasePriceUsd": 1250.50,
    "OfficeLocation": "Sweden Office",
    "SerialNumber": "LEN-T14-001",
    "EmployeeUsername": "ephraim",
    "WarrantyExpirationDate": "2028-01-15"
  }
]
```

To import multiple assets, add more objects inside the array,
separated by commas.

Do not add a comma after the last object.

### Fields and Validation

Property names are case-sensitive. Use the exact spelling shown
in the example: AssetType, not assetType.

| Field | Required | Description |
| --- | --- | --- |
| AssetType | Yes | Exactly Laptop, Desktop, or Mobile Phone |
| Brand | Yes | Non-empty text identifying the manufacturer |
| ModelName | Yes | Non-empty text identifying the model |
| PurchaseDate | Yes | Valid date in yyyy-MM-dd format |
| PurchasePriceUsd | Yes | Non-negative JSON number in USD; use a decimal point, without quotation marks or a currency symbol |
| OfficeLocation | Yes | Non-empty office-location text |
| SerialNumber | Yes | Non-empty serial-number text |
| EmployeeUsername | No | Text; may be omitted, set to null, or left empty |
| WarrantyExpirationDate | Yes | Valid date in yyyy-MM-dd format, on or after the purchase date |

Do not include an Id. The reader creates new entities, and the
database generates their IDs when they are saved.

Laptop and Desktop entries create ComputerAsset objects.
Mobile Phone entries create MobileAsset objects.

Importing an asset does not create or validate a separate
office record.

### Entering the File Path

Select option 5 and enter the path to your JSON file:

```text
JSON file path: C:\Users\userName\Documents\assets.json
```

Replace userName with the appropriate Windows user folder,
or enter the actual location of your file.

Use ordinary Windows backslashes exactly as shown.
Do not double them, and do not add a C# @ prefix.

The path is entered as console text, so C# string escaping
does not apply.

Paths pasted using Windows Copy as path are also supported:

```text
JSON file path: "C:\Users\userName\Documents\assets.json"
```

The application removes surrounding quotation marks.
Paths containing spaces are supported.

These rules apply to the path entered at the console.
Backslashes inside a JSON string follow JSON escaping rules.

### Import Behaviour

1. AssetFileReader reads the file.
2. It parses the JSON and validates every entry.
3. An empty file, empty array, invalid JSON, or invalid entry
   cancels the import before saving.
4. ImportAssetsAction passes valid assets to the repository.
5. The repository saves the batch together.
6. The application displays the number of imported assets.

For the example above, a successful import displays:

```text
1 assets imported successfully.
```

Validation errors identify the affected entry where possible:

```text
Import cancelled: Asset entry 1: AssetType must be Laptop, Desktop, or Mobile Phone.
```

### Duplicate Handling

The asset import reader and action do not check for duplicates.

Importing the same file again attempts to insert its assets again.
Existing assets are not updated by the import.

## Error Handling

The application provides messages for:

- Invalid or missing required input.
- Invalid JSON and invalid asset entries.
- Files that cannot be read.
- Invalid or unsupported file paths.
- File access permission errors.
- Changes that cannot be saved to the database.
- SQL Server connection or database errors.
- Action cancellation and ended console input.

Handled file-reading and validation failures return to the menu.

ActionCancelledException returns to the menu when asset ID
selection is cancelled.

Database errors handled by Program.cs end the current run.

## Verified Functionality

The following features have been tested manually:

- Creating, displaying, updating, and deleting assets.
- Importing assets from JSON.
- Displaying assets by category.
- Displaying the aligned asset table with lifetime colours
  and a legend below it.

Shared cancellation behaviour was also tested through
the Level 3 application.

## Current Limitations

- Office locations are free text.
- Asset import does not detect duplicates.
- Employee assignment is a username field, not a relationship
  to an employee record.
- Prices are stored in USD; Level 2 does not convert currencies.
- Database errors handled at application level end the run.

## Next Level

Level 3 extends the application with:

- Registered offices containing a name, country, and currency.
- Office JSON import with duplicate checking.
- Registered-office selection for asset creation and editing.
- A separate asset display with office and price columns.
- Conversion from USD using fixed example exchange rates.
- Asset counts and purchase-price totals per office.
- Reports for expiring and expired assets.
- Reports for the most expensive assets.