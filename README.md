# Smart Asset Tracking — All Projects

A C# console application developed in five progressive levels to manage IT assets, offices, employees, and maintenance. Each level extends the previous one through project references and reuses shared components.

## Start here: run the projects

**[Read HOW_TO_RUN_PROJECTS.md](HOW_TO_RUN_PROJECTS.md)** for step-by-step instructions covering:

- Cloning the GitHub repository and opening the solution.
- Restoring packages and building the projects.
- Configuring SQL Server and applying the existing migrations.
- Running and checking each of the five levels.
- Using the Level 5 demo login and troubleshooting setup problems.

The guide includes a checklist for teachers and reviewers.

## Project levels

| Project | Capabilities added | Documentation |
| --- | --- | --- |
| **Level 1** | Add and display assets, input validation, lifetime highlighting, database persistence | [Level 1 README](AssetTracking.Level1/README.md) |
| **Level 2** | Update and delete assets, JSON asset import, category views | [Level 2 README](AssetTracking.Level2/README.md) |
| **Level 3** | Office management, office JSON import, local currency display, office totals, expiration and price reports | [Level 3 README](AssetTracking.Level3/README.md) |
| **Level 4** | Combined search and filtering, report export to TXT, CSV, and JSON | [Level 4 README](AssetTracking.Level4/README.md) |
| **Level 5** | Simulated role-based login, employees, asset assignments, maintenance history, management dashboard | [Level 5 README](AssetTracking.Level5/README.md) |

Each project is a runnable console application. Select one startup project at a time in Visual Studio; the earlier applications do not need to run in the background.

## Technologies

- C# and .NET 10.
- Entity Framework Core 10 with SQL Server.
- LINQ for queries, filtering, and reporting.
- System.Text.Json for JSON import and export.
- Visual Studio 2026.

## Design approach

The solution is organised around SOLID principles and separation of responsibilities:

- **Models** describe assets, offices, employees, and maintenance records.
- **Data** contains EF Core contexts, design-time factories, and migrations.
- **Repositories** provide asset and office persistence operations.
- **Services** handle search, currency conversion, assignments, maintenance, and dashboard calculations.
- **ConsoleUI** collects input and presents results.
- **Menu actions** implement individual operations through the shared `IMenuAction` interface.
- **Program.cs** in each project configures dependencies and registers that level's capabilities.

Interfaces allow shared actions to work with different display and office-input implementations. New menu actions can be registered without changing the menu's selection logic.

## Database setup

All five levels use the same SQL Server database, `asset_tracking`, with Windows Authentication. The default server is `localhost\SQLEXPRESS`.

The repository contains migrations, not a copy of the database. A fresh clone requires applying the existing migrations in this order:

1. **Level 1 — AssetContext:** creates the Assets table.
2. **Level 3 — OfficeAssetContext:** adds the Offices table.
3. **Level 5 — EnterpriseContext:** adds Employees and MaintenanceRecords.

Levels 2 and 4 introduce no additional migrations. See the [running guide](HOW_TO_RUN_PROJECTS.md#5-create-the-database-using-the-existing-migrations) for the exact commands and connection settings.

Data persists between runs. Changes made in one level are visible in the other levels.

## Sample data and login

- [`asus-assets.json`](asus-assets.json) contains an example desktop asset that can be imported through option **5** in Level 2 or higher.
- On first use of Level 5, an empty Employees table receives a demonstration administrator with username **admin**.
- Login uses registered usernames without passwords. It is an educational simulation, not production authentication.
- Register an office before manually adding assets in Levels 3–5.

## Asset lifetime and currency

Assets have an expected lifetime of **three years from purchase**. The current display uses:

| Remaining lifetime | Display colour |
| --- | --- |
| Expired or less than three months | Red |
| Three months to less than six months | Yellow |
| Six months or more | Normal |

Lifetime highlighting is independent of warranty expiration. Currency conversion uses fixed educational exchange rates, not live financial data.

## Repository entry points

| File or folder | Purpose |
| --- | --- |
| `SmartAssetTracking.slnx` | Open this solution in Visual Studio |
| `HOW_TO_RUN_PROJECTS.md` | Complete setup and review walkthrough |
| `AssetTracking.Level1` through `AssetTracking.Level5` | Source code and level-specific READMEs |
| `asus-assets.json` | Example asset import file |

Start with the [running guide](HOW_TO_RUN_PROJECTS.md), then use each level's README for implementation details and limitations.
