# Smart Asset Tracking — Level 5

Level 5 extends Levels 1–4 with employee management, simulated
login, asset assignment, maintenance tracking, and a dashboard.

## Features

- Username-only login simulation.
- Admin, Manager, and Employee roles.
- Employee registration and listing.
- Asset assignment and unassignment.
- Maintenance recording and history.
- Management dashboard.
- Reuse of asset CRUD, offices, reporting, search, and exports
  from earlier levels.

## Roles

| Feature | Admin | Manager | Employee |
|---|---|---|---|
| Inherited management options 1–13 | Yes | Yes | No |
| List employees | Yes | Yes | No |
| Register employees | Yes | No | No |
| Assign and unassign assets | Yes | Yes | No |
| View employee assignments | All | All | Own only |
| Record maintenance | Yes | Yes | No |
| View maintenance history | All assets | All assets | Own assets |
| View management dashboard | Yes | Yes | No |

Login is an educational simulation. It uses registered usernames
without passwords and is not production authentication.

On startup, an empty employee database receives a demonstration
administrator with username `admin`.

To switch users, exit the application and run it again.

## New Menu Options

- **14 — Employees and asset assignments**
- **15 — Asset maintenance**
- **16 — Management dashboard**

Employees see only options 14, 15, and 0.
Option 14 displays their own assignments directly.

## Employee Management

Each employee contains:

- Id.
- Username.
- Full name.
- Department.
- Email.
- Role.

Usernames are trimmed, stored in lowercase, and must be unique.

Assignments use the existing `Asset.EmployeeUsername` field.
Assigned assets are retrieved through `EmployeeService`.

An asset assigned to another username must be unassigned before
it can be reassigned.

## Maintenance Tracking

Each maintenance record contains:

- Id.
- Asset Id.
- Last maintenance date.
- Next maintenance date.
- Notes.

Validation checks that:

- The asset exists.
- Completed maintenance is not before purchase or in the future.
- The next maintenance date is after the last maintenance date.
- Notes contain 1–2000 characters.

History is displayed with the most recent maintenance first.

## Dashboard

The dashboard displays:

- Total assets.
- Total purchase value in USD.
- Expired assets.
- Assets expiring within the next six months.
- Unassigned assets.
- Most common registered asset type, including ties.
- Purchase value by office in USD.
- Asset count per registered employee.

Expiration uses a three-year lifetime from the purchase date.

OfficeResolver combines alternative labels such as `Sweden`
and `Sweden Office` when they match one registered office.

Unresolved office labels remain separate.

“Most used asset type” means the type with the highest registered
asset count, not measured usage.

## Project Structure

### Models

- `Employee.cs`
- `UserRole.cs`
- `MaintenanceRecord.cs`

### Data

- `EnterpriseContext.cs`
- `EnterpriseContextFactory.cs`
- `Migrations/`

### Services

- `EmployeeService.cs`
- `MaintenanceService.cs`
- `LoginService.cs`
- `DashboardService.cs`

### ConsoleUI

- `EnterpriseInput.cs`
- `LoginScreen.cs`

### ConsoleUI/Actions

- `ManageEmployeesAction.cs`
- `ManageMaintenanceAction.cs`
- `DisplayDashboardAction.cs`

### Application Entry Point

- `Program.cs` configures contexts, services, login, and menu actions.

## Database Setup

### Prerequisites

- .NET 10.
- SQL Server Express.
- Existing database setup from Levels 1–3.
- A project reference from Level 5 to Level 4.
- EF Core SQL Server support.
- EF Core Package Manager Console tools.
- `Microsoft.EntityFrameworkCore.Design` installed in Level 5.

Keep EF Core package versions consistent across the solution.

### Connection

Server: `localhost\SQLEXPRESS`

Database: `asset_tracking`

Authentication: Windows Authentication.

### Contexts

`OfficeAssetContext` manages existing assets and offices.

`EnterpriseContext` manages:

- `Employees`
- `MaintenanceRecords`

Level 5 uses its own migration-history table:

`__EFMigrationsHistory_Level5`

The runtime configuration and design-time factory use the same
connection and migration-history settings.

### Create the Migration

Run this in Visual Studio's Package Manager Console when creating
the migration for the first time:

```powershell
Add-Migration AddEnterpriseTables -Context EnterpriseContext -Project AssetTracking.Level5 -StartupProject AssetTracking.Level5 -OutputDir Data/Migrations
```

If `AddEnterpriseTables` already exists in the project, do not
create it again.

### Apply the Migration

```powershell
Update-Database -Context EnterpriseContext -Project AssetTracking.Level5 -StartupProject AssetTracking.Level5
```

This applies pending Level 5 migrations to the database.

## Running

1. Ensure SQL Server is running.
2. Apply the required migrations.
3. Set `AssetTracking.Level5` as the startup project.
4. Build and run.
5. On the first run, log in as `admin`.
6. Use option 14 to register employees and assign assets.
7. Use option 15 for maintenance.
8. Use option 16 for dashboard statistics.
9. Enter 0 to exit.

## Validation Completed

- Level 5 migrations applied successfully.
- Admin login displayed all management options.
- Employee registration and asset assignment worked.
- The dashboard reflected the employee assignment.
- Office aliases were combined into one registered office.
- Employee login displayed the restricted menu and assigned asset.

## Implementation Limits

- Login does not verify passwords.
- Employee assignments are username-based rather than foreign keys.
- Existing asset workflows can contain usernames that are not
  registered employees.
- Maintenance AssetId is validated by the service; it is not
  enforced by a database foreign key.
- Deleting an asset does not automatically delete its maintenance
  records.
- Dashboard purchase values are not depreciated values.

## Error Handling

Input validation reports invalid values.
Database exceptions are handled at the application entry point.

The SQL exception handler displays the SQL error number and
message to support diagnosis during development.