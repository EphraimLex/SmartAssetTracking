# Smart Asset Tracking — Level 1

A C# console application for registering company assets, saving them
to SQL Server, and displaying them in a table.

Level 1 also provides shared components that later levels reuse
and extend.

## Technologies

- C# and .NET 10
- Entity Framework Core 10
- SQL Server Express
- LINQ

## Features

- Register laptops, desktop computers, and mobile devices.
- Save assets to a SQL database.
- Display assets in an aligned console table.
- Highlight assets according to their remaining lifetime.
- Validate required fields, dates, and prices.
- Allow an optional employee assignment.
- Handle database errors with readable messages.

## Asset Information

Each asset contains:

| Property | Description |
| --- | --- |
| Id | Database-generated identifier |
| AssetType | Asset type, such as Laptop, Desktop, or Mobile Phone |
| Brand | Manufacturer or brand |
| ModelName | Model name |
| PurchaseDate | Date of purchase |
| PurchasePriceUsd | Purchase price in USD |
| OfficeLocation | Office location stored as text |
| SerialNumber | Asset serial number |
| EmployeeUsername | Optional assigned employee |
| WarrantyExpirationDate | Warranty expiration date |

The current Level 1 table displays:

- ID
- Brand
- Model
- Purchase date
- Lifetime status

The remaining information is also stored in the database.

## Lifetime Colours

Expected asset lifetime is three years from the purchase date.

| Remaining lifetime | Colour |
| --- | --- |
| Expired or less than 3 months | Red |
| 3 months to less than 6 months | Yellow |
| 6 months or more | Normal |

The most urgent condition is checked first.

Rows are coloured according to their status. The colour
explanation appears once below the complete table.

Lifetime status is separate from warranty expiration.

## Project Organization

| Folder | Responsibility |
| --- | --- |
| Models | Asset properties, constructors, and inheritance |
| Data | EF Core context and design-time factory |
| Repositories | Database access and persistence |
| ConsoleUI | Menu, input validation, and table displays |
| ConsoleUI/Actions | Individual menu operations |
| Migrations | Database schema migrations |

Program.cs configures the database connection, creates the
application components, registers menu actions, and handles
application-level errors.

## Design

### Inheritance

Asset is an abstract base class containing shared properties.

- ComputerAsset represents laptops and desktops.
- MobileAsset represents mobile devices.

EF Core stores these types in the Assets table using
table-per-hierarchy inheritance.

A discriminator column identifies the entity type.
The separate AssetType property records values such as
Laptop, Desktop, or Mobile Phone.

Asset provides two protected constructors:

- A parameterless constructor for derived classes and EF Core.
- A constructor that initializes the shared asset properties.

### Separation of Responsibilities

- ConsoleInput reads and validates individual input values.
- AssetInput collects asset details and handles property edits.
- AssetRepository handles asset database operations.
- AssetDisplay formats the asset table.
- ConsoleMenu displays and executes registered actions.
- Individual action classes coordinate each menu operation.

### Interfaces and Dependency Injection

IAssetRepository defines asset database operations.

IMenuAction defines a menu option through its Key, Description,
and Execute method.

IAssetDisplay defines two display operations:

- DisplayAssets
- DisplayByCategory

IOfficeLocationInput defines how an office location is obtained.

Dependencies are supplied through constructors and assembled
in each project's Program.cs.

### Behaviour by Level

Each level chooses its menu actions and implementations.

| Component | Level 1 behaviour | Extension example |
| --- | --- | --- |
| ConsoleMenu | Runs registered actions | Later levels register additional actions |
| IAssetDisplay | AssetDisplay provides the existing table | Level 3 supplies OfficeAssetDisplay |
| IOfficeLocationInput | TextOfficeLocationInput accepts typed locations | Level 3 selects registered offices |

AssetInput retains its original constructor for free-text office
entry. A second constructor accepts an IOfficeLocationInput
implementation.

This allows later levels to change office input without adding
a dependency from Level 1 to Level 3.

The project can contain shared action classes that are not
registered in Level 1's menu. Available options are determined
by Program.cs, not by the presence of those classes.

## Database

The default database configuration is:

- Server: localhost\SQLEXPRESS
- Database: asset_tracking
- Authentication: Windows Authentication

Connection string:

```text
Server=localhost\SQLEXPRESS;Database=asset_tracking;Integrated Security=True;TrustServerCertificate=True;
```

AssetContext provides access to assets and configures:

- ComputerAsset and MobileAsset inheritance.
- USD purchase prices with decimal precision of 18,2.

Its public constructor accepts DbContextOptions<AssetContext>.

A protected constructor accepts non-generic DbContextOptions,
allowing a derived context in a later level to reuse the asset
configuration.

Level 1 migrations manage the Assets table. Applied migrations
are recorded in __EFMigrationsHistory.

## Setup

1. Install the .NET 10 SDK and SQL Server Express.
2. Ensure the project references the required EF Core packages:
   - Microsoft.EntityFrameworkCore.SqlServer
   - Microsoft.EntityFrameworkCore.Tools
   - Microsoft.EntityFrameworkCore.Design
3. Use matching EF Core 10.0.x package versions.
4. Check the connection strings in Program.cs and
   Data/AssetContextFactory.cs.
5. Set AssetTracking.Level1 as the startup project.
6. Open Visual Studio's Package Manager Console.
7. Apply the existing migrations:

```powershell
Update-Database -Context AssetContext -Project AssetTracking.Level1 -StartupProject AssetTracking.Level1
```

FirstMigration is already included. There is no need to create
it again.

AssetContextFactory supplies an AssetContext to EF Core's
migration tools.

## Usage

The basic Level 1 menu provides:

| Option | Action |
| --- | --- |
| 1 | Add asset |
| 2 | Display all assets |
| 0 | Exit |

The actual menu reflects the actions registered in Level 1's
Program.cs.

When adding a computer, select Laptop or Desktop.

Enter dates in yyyy-MM-dd format:

```text
2026-01-15
```

Enter prices using a decimal point:

```text
1500.00
```

Enter the office location as free text.

Press Enter at the employee prompt to leave the employee
unassigned.

## Shared Cancellation Support

Shared components support cancellation for actions used by
later levels:

- Entering 0 at an asset ID prompt cancels the current action.
- ActionCancelledException returns control to the menu.
- OfficeSelectionException returns control to the menu when
  registered-office selection is cancelled or unavailable.

Entering 0 at the main menu exits the application.

Cancellation is supported at these specific prompts; it is not
a universal command for every input field.

## Validation and Error Handling

- Required text cannot be blank.
- Dates must use yyyy-MM-dd.
- Prices must be non-negative.
- Warranty expiration cannot precede the purchase date.
- Asset IDs must be positive integers, with 0 reserved for cancellation.
- Invalid input repeats the relevant prompt.
- Handled database failures display a readable message and
  end the application.
- Ending console input closes the application through the
  OperationCanceledException handler.

## Verification Performed

Earlier Level 1 checks included:

- Adding and displaying a Lenovo computer.
- Adding an Asus desktop and checking its asset type.
- Testing database error handling with a nonexistent database.
- Restoring the connection and confirming asset retrieval.

Shared input and display components have also been exercised
through the later-level applications.

## Scope

Level 1's basic menu provides asset creation and reading.
Its current display includes lifetime colours.

The project also contains reusable components used by later
levels, including repository operations, input validation,
menu actions, and display interfaces.

Later levels build on these components:

- Level 2: full CRUD, JSON asset import, and category views.
- Level 3: offices, currency conversion, and reporting.
- Level 4: advanced search, filtering, and export.
- Level 5: employee assignment, maintenance, and further
  management features.

Each level's Program.cs controls its available capabilities.