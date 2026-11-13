namespace AssetTracking.Level5.Models;

// Stores an employee's identity and simulated application role.
// Username links the employee to Asset.EmployeeUsername.
public class Employee
{
    public int Id { get; set; }

    public string Username { get; set; } = "";

    public string FullName { get; set; } = "";

    public string Department { get; set; } = "";

    public string Email { get; set; } = "";

    public UserRole Role { get; set; } = UserRole.Employee;
}