using AssetTracking.Level5.Models;

namespace AssetTracking.Level5.Services;

// Simulates login by selecting a registered employee identity.
public class LoginService
{
    private readonly EmployeeService _employees;

    public LoginService(EmployeeService employees)
    {
        _employees = employees;
    }

    public Employee? CurrentUser { get; private set; }

    public void EnsureDemoAdmin()
    {
        // Only initializes an empty employee database.
        if (_employees.GetAll().Count > 0)
        {
            return;
        }

        _employees.Add(new Employee
        {
            Username = "admin",
            FullName = "Demo Administrator",
            Department = "IT",
            Email = "admin@example.com",
            Role = UserRole.Admin
        });
    }

    public bool Login(string username)
    {
        CurrentUser = _employees.GetByUsername(username);

        return CurrentUser is not null;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}