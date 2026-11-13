using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level5.Models;
using AssetTracking.Level5.Services;

namespace AssetTracking.Level5.ConsoleUI.Actions;

// Handles employee registration and asset assignment.
public class ManageEmployeesAction : IMenuAction
{
    private readonly ConsoleInput _input;
    private readonly EmployeeService _employees;
    private readonly Employee _currentUser;

    public ManageEmployeesAction(
        ConsoleInput input,
        EmployeeService employees,
        Employee currentUser)
    {
        _input = input;
        _employees = employees;
        _currentUser = currentUser;
    }

    public string Key => "14";

    public string Description => "Employees and asset assignments";

    public void Execute()
    {
        // Employees may view only their own assignments here.
        if (_currentUser.Role == UserRole.Employee)
        {
            DisplayAssignments(_currentUser.Username);
            return;
        }

        Console.WriteLine();
        Console.WriteLine("1. List employees");
        Console.WriteLine("2. Register employee (Admin)");
        Console.WriteLine("3. Assign asset");
        Console.WriteLine("4. Unassign asset");
        Console.WriteLine("5. View employee assets");
        Console.WriteLine("0. Return");

        int choice = EnterpriseInput.ReadNumber(
            _input, "Select: ", 0, 5);

        try
        {
            switch (choice)
            {
                case 1:
                    DisplayEmployees();
                    break;

                case 2:
                    RegisterEmployee();
                    break;

                case 3:
                    AssignAsset();
                    break;

                case 4:
                    UnassignAsset();
                    break;

                case 5:
                    string username = _input.ReadRequiredText(
                        "Employee username: ");

                    if (_employees.GetByUsername(username) is null)
                    {
                        Console.WriteLine("Employee not found.");
                        return;
                    }

                    DisplayAssignments(username);
                    break;
            }
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    private void DisplayEmployees()
    {
        var employees = _employees.GetAll();

        Console.WriteLine();

        foreach (var employee in employees)
        {
            Console.WriteLine(
                $"{employee.Id} | {employee.Username} | " +
                $"{employee.FullName} | {employee.Department} | " +
                $"{employee.Email} | {employee.Role}");
        }

        Console.WriteLine($"Employees: {employees.Count}");
    }

    private void RegisterEmployee()
    {
        if (_currentUser.Role != UserRole.Admin)
        {
            Console.WriteLine("Only Admin can register employees.");
            return;
        }

        Employee employee = new()
        {
            Username = _input.ReadRequiredText("Username: "),
            FullName = _input.ReadRequiredText("Full name: "),
            Department = _input.ReadRequiredText("Department: "),
            Email = _input.ReadRequiredText("Email: ")
        };

        Console.WriteLine("Role: 0 = Admin, 1 = Manager, 2 = Employee");

        employee.Role = (UserRole)EnterpriseInput.ReadNumber(
            _input, "Role: ", 0, 2);

        _employees.Add(employee);

        Console.WriteLine($"Employee created with ID {employee.Id}.");
    }

    private void AssignAsset()
    {
        int assetId = EnterpriseInput.ReadNumber(
            _input, "Asset ID: ", 1);

        string username = _input.ReadRequiredText(
            "Employee username: ");

        _employees.AssignAsset(assetId, username);

        Console.WriteLine("Asset assigned successfully.");
    }

    private void UnassignAsset()
    {
        int assetId = EnterpriseInput.ReadNumber(
            _input, "Asset ID: ", 1);

        _employees.UnassignAsset(assetId);

        Console.WriteLine("Asset unassigned successfully.");
    }

    private void DisplayAssignments(string username)
    {
        var assets = _employees.GetAssignedAssets(username);

        Console.WriteLine();
        Console.WriteLine($"Assets assigned to {username}:");

        foreach (var asset in assets)
        {
            Console.WriteLine(
                $"{asset.Id} | {asset.AssetType} | " +
                $"{asset.Brand} | {asset.ModelName}");
        }

        Console.WriteLine($"Assigned assets: {assets.Count}");
    }
}