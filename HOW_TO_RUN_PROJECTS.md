# How to Run Smart Asset Tracking

Step-by-step guide for teachers and reviewers, from cloning the repository to checking all five console projects.

## 1. Install the prerequisites

Use a Windows computer with:

- **Visual Studio 2026**, with the **.NET desktop development** workload and **.NET 10 SDK** installed.
- **SQL Server Express**, with a running instance named **SQLEXPRESS**. The project was developed with SQL Server 2025 Express.
- **Git**, if cloning from a terminal.
- Internet access to clone the repository and restore NuGet packages.

SQL Server Management Studio is optional. Installing a database management tool alone does not install the SQL Server database engine.

The applications use Windows Authentication. Your Windows account must be allowed to connect to SQL Server and create/update the application database.

To check the SDK, open a terminal and run:

```powershell
dotnet --list-sdks
```

The output should include a `10.0.x` SDK.

## 2. Clone the repository

In Visual Studio:

1. Open Visual Studio and select **Clone a repository**.
2. Enter this repository URL:

   ```text
   https://github.com/EphraimLex/SmartAssetTracking.git
   ```

3. Choose a local destination and select **Clone**.
4. Open **SmartAssetTracking.slnx** from the cloned folder if it does not open automatically. Use **File > Open > Project/Solution** if necessary.

Alternatively, clone from a terminal:

```powershell
git clone https://github.com/EphraimLex/SmartAssetTracking.git
cd SmartAssetTracking
```

Then open `SmartAssetTracking.slnx` in Visual Studio.

## 3. Check the solution and restore packages

Solution Explorer should contain these five projects:

| Project | Main additions |
| --- | --- |
| AssetTracking.Level1 | Add and display assets |
| AssetTracking.Level2 | Update, delete, JSON import, category views |
| AssetTracking.Level3 | Offices, local currency display, reports |
| AssetTracking.Level4 | Search, filtering, TXT/CSV/JSON export |
| AssetTracking.Level5 | Simulated login, employees, assignments, maintenance, dashboard |

Each level references the previous level. Run one startup project at a time; earlier applications do not need to be running in the background.

1. Right-click the solution and select **Restore NuGet Packages**, or allow the automatic restore to finish.
2. Select **Build > Build Solution** (`Ctrl+Shift+B`).
3. Check that the build succeeds before continuing.

The required EF Core packages are already declared in the project files. Restore those versions rather than installing packages manually.

## 4. Check the SQL Server connection

The default connection is:

```text
Server=localhost\SQLEXPRESS;Database=asset_tracking;Integrated Security=True;TrustServerCertificate=True;
```

1. Open Windows **Services** and check that **SQL Server (SQLEXPRESS)** is running.
2. If your server is `localhost\SQLEXPRESS`, keep the existing connection strings.
3. If your server has another name, change the `Server=` value in **all eight files** below:

| Runtime connections | Migration connections |
| --- | --- |
| AssetTracking.Level1/Program.cs | AssetTracking.Level1/Data/AssetContextFactory.cs |
| AssetTracking.Level2/Program.cs | AssetTracking.Level3/Data/OfficeAssetContextFactory.cs |
| AssetTracking.Level3/Program.cs | AssetTracking.Level5/Data/EnterpriseContextFactory.cs |
| AssetTracking.Level4/Program.cs | |
| AssetTracking.Level5/Program.cs | |

For example, a default SQL Server instance may use `Server=localhost;`. Use the actual instance installed on your computer.

All runtime and migration connections must point to the **same database**. Keep the existing migration-history settings unchanged.

The database is not included in the Git clone. The next step creates its tables. If your SQL Server already contains an unrelated database named `asset_tracking`, choose a new database name and change it consistently in all eight files before continuing.

## 5. Create the database using the existing migrations

Open **Tools > NuGet Package Manager > Package Manager Console** in Visual Studio.

Run the following commands **one at a time, in this order**. Wait for each command to finish successfully before running the next.

### A. Create the database and Assets table — Level 1

```powershell
Update-Database -Context AssetContext -Project AssetTracking.Level1 -StartupProject AssetTracking.Level1
```

### B. Add the Offices table — Level 3

```powershell
Update-Database -Context OfficeAssetContext -Project AssetTracking.Level3 -StartupProject AssetTracking.Level3
```

### C. Add the Employees and MaintenanceRecords tables — Level 5

```powershell
Update-Database -Context EnterpriseContext -Project AssetTracking.Level5 -StartupProject AssetTracking.Level5
```

These are **Package Manager Console commands**, not ordinary PowerShell terminal commands. The explicit project arguments avoid depending on the console's Default project selection.

The migration files are already committed. **Do not run Add-Migration for this setup.** Levels 2 and 4 need no additional migrations. Applying all three sets of migrations prepares the shared database for all five applications.

Expected application tables: `Assets`, `Offices`, `Employees`, and `MaintenanceRecords`. EF Core also creates migration-history tables. A message that the database is already up to date is normal when repeating setup.

## 6. Run and check Level 1

1. Right-click **AssetTracking.Level1** in Solution Explorer.
2. Select **Set as Startup Project**.
3. Press **Ctrl+F5** to run without debugging.
4. Select **1 — Add asset** and enter the following sample:

   | Prompt | Input |
   | --- | --- |
   | Asset type | `1` — Computer |
   | Computer type | `1` — Laptop |
   | Brand | `Lenovo` |
   | Model name | `Teacher Test Laptop` |
   | Purchase date | `2026-01-15` |
   | Purchase price (USD) | `1000` |
   | Office location | `Sweden Office` |
   | Serial number | `TEACHER-TEST-001` |
   | Warranty expiration date | `2029-01-15` |
   | Employee username | Press Enter to leave empty |

5. Select **2** to display assets. Confirm the new asset appears and note its ID.
6. Enter **0** at the main menu to exit.

An empty asset list before adding data is normal. Widen the console window if table columns wrap. Lifetime colours depend on the current date and a three-year asset lifetime, independently of warranty dates.

## 7. Run and check Level 2

1. Set **AssetTracking.Level2** as the startup project and press **Ctrl+F5**.
2. Select **2**. The Level 1 asset should still be present because the database is shared.
3. Select **3**, enter its actual ID, and update one property, such as the model name. Display the list again to confirm the change.
4. Select **5** to import assets. Enter the full path to the included `asus-assets.json` file, for example:

   ```text
   C:\Users\YourName\source\repos\SmartAssetTracking\asus-assets.json
   ```

   Replace the example with your actual clone location. The database generates the imported asset's ID; do not assume it is the ID shown in the JSON file.

5. Select **2** and confirm that an ASUS ExpertCenter D7 asset appears.
6. Select **6** to check the category view.
7. To check deletion, add a separate disposable asset, note its ID, then use **4** and follow the confirmation prompt. Keep the Lenovo and ASUS assets for later checks.
8. Enter **0** to exit.

## 8. Run and check Level 3

1. Set **AssetTracking.Level3** as the startup project and press **Ctrl+F5**.
2. Select **7** to add an office:

   | Prompt | Input |
   | --- | --- |
   | Office name | `Sweden Office` |
   | Country | `Sweden` |
   | Currency | `SEK` |

3. Select **8** and confirm the office appears. If repeating the walkthrough, use the existing office instead of adding it again.
4. Select **2** to display assets with office and local-currency information.
5. Select **10** to check asset totals by office.
6. Select **11** for expiring/expired assets and **12** for the most expensive assets. An empty expiration report can be correct for your data and today's date.
7. Option **1** now selects from registered offices. Add an asset if you want to check this input flow.
8. Enter **0** to exit.

Register at least one office before manually adding assets in Levels 3–5. Currency conversion uses fixed educational rates, not live rates. Option **9** supports importing an office JSON file; manual creation above is sufficient for this walkthrough.

Current repository note: Level 3 registers the option 11 action twice, so it may appear twice in the menu. Both entries refer to the same action.

## 9. Run and check Level 4

1. Set **AssetTracking.Level4** as the startup project and press **Ctrl+F5**.
2. Select **13 — Search, filter, and export assets**.
3. Enter `ASUS` for the brand.
4. Press Enter to skip model, office, and purchase year.
5. Enter **0** for all categories and **n** for expired-only filtering.
6. Confirm that the imported ASUS asset appears.
7. Choose **2** for CSV export and enter a full path to a **new file in an existing writable folder**, ending in `.csv`.
8. Open the exported file and check its contents.
9. Repeat option **13** to test **1 — TXT** and **3 — JSON**, using new filenames with the matching extensions. Choose **0** at the export prompt to return without exporting.
10. Enter **0** at the main menu to exit.

Search criteria combine: an asset must match all supplied criteria. If no assets match, the action returns without offering export. Existing output files are not overwritten.

## 10. Run and check Level 5

1. Set **AssetTracking.Level5** as the startup project and press **Ctrl+F5**.
2. At the first login, enter **admin**. An empty Employees table is automatically given this demo administrator. No password is required: login is an educational simulation.
3. Select **14**, then **2**, to register a test employee. For example:

   | Prompt | Input |
   | --- | --- |
   | Username | `teacher.test` |
   | Full name | `Teacher Test` |
   | Department | `IT` |
   | Email | `teacher.test@example.com` |
   | Role | `2` — Employee |

4. Select **2** from the main menu to find an unassigned asset's actual ID.
5. Select **14**, then **3**, and assign that asset to `teacher.test`.
6. Select **14**, then **5**, to view that employee's assets.
7. Select **15** and follow the maintenance submenu to record and view maintenance for that asset. Use a completed maintenance date on or after purchase and no later than today; the next maintenance date must be later than the completed date.
8. Select **16** and check the dashboard totals.
9. Exit with **0**, run Level 5 again, and log in as **teacher.test**.
10. Confirm that the employee menu contains **14**, **15**, and **0**, with access to their own assignments and maintenance history.

Exit and restart to switch users. The demo administrator is created only when the Employees table is empty. If reusing an existing database, use an already registered username.

## 11. Troubleshooting

| Symptom | Check or action |
| --- | --- |
| .NET 10 target is unsupported | Install the .NET 10 SDK and use a Visual Studio version supporting it. |
| NuGet restore fails | Check internet access and NuGet source settings, then restore again. |
| Update-Database is not recognised | Use Visual Studio's Package Manager Console, finish package restore, then reopen the console if necessary. |
| SQL Server cannot be reached | Check the SQL Server service, actual instance name, and all connection strings. |
| Cannot open database / login failed | Check Windows-account permissions and that migrations target the same server and database as the application. |
| Invalid object name, such as Assets or Offices | Apply the migration commands in Step 5 in order. |
| No offices available | Run Level 3 or higher and add an office with option 7. |
| Import file not found | Enter the full path to the file in your cloned folder. |
| Export fails | Use an existing writable folder, a new filename, and the required extension. |
| Wrong menu or Hello, World! appears | Check the selected startup project and its Program.cs, then rebuild and run again. |
| Level 5 username not found | Use admin on a fresh database, or a username registered in this database. |

All five projects share saved data: updates and deletions in one level are visible in the others. Closing an application does not clear the database.

## Review checklist

- [ ] Solution cloned, opened, restored, and built.
- [ ] All three migration commands completed successfully.
- [ ] Level 1: add and display an asset.
- [ ] Level 2: update, delete, JSON import, and category view.
- [ ] Level 3: register an office and inspect currency display/reports.
- [ ] Level 4: search and export TXT, CSV, and JSON.
- [ ] Level 5: admin login, employee registration, assignment, maintenance, dashboard, and employee view.

For implementation details, see the README.md inside each project folder.
