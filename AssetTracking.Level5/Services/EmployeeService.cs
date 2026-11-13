using AssetTracking.Level1.Models;
using AssetTracking.Level1.Repositories;
using AssetTracking.Level5.Data;
using AssetTracking.Level5.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace AssetTracking.Level5.Services;

// Manages employees and links assets through EmployeeUsername.
public class EmployeeService
{
    private readonly EnterpriseContext _context;
    private readonly IAssetRepository _assets;

    public EmployeeService(
        EnterpriseContext context,
        IAssetRepository assets)
    {
        _context = context;
        _assets = assets;
    }

    public IReadOnlyList<Employee> GetAll()
    {
        return _context.Employees
            .AsNoTracking()
            .OrderBy(employee => employee.FullName)
            .ToList();
    }

    public Employee? GetByUsername(string username)
    {
        string normalizedUsername = username.Trim().ToLowerInvariant();

        return _context.Employees
            .AsNoTracking()
            .FirstOrDefault(
                employee => employee.Username == normalizedUsername);
    }

    public void Add(Employee employee)
    {
        employee.Username = employee.Username.Trim().ToLowerInvariant();
        employee.FullName = employee.FullName.Trim();
        employee.Department = employee.Department.Trim();
        employee.Email = employee.Email.Trim();

        ValidateText(employee.Username, 100, "Username");
        ValidateText(employee.FullName, 150, "Full name");
        ValidateText(employee.Department, 100, "Department");
        ValidateText(employee.Email, 254, "Email");

        if (employee.Username.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException(
                "Username cannot contain spaces.");
        }

        if (!MailAddress.TryCreate(employee.Email, out var address)
            || address.Address != employee.Email)
        {
            throw new ArgumentException("Enter a valid email address.");
        }

        if (!Enum.IsDefined(typeof(UserRole), employee.Role))
        {
            throw new ArgumentException("Select a valid role.");
        }

        if (GetByUsername(employee.Username) is not null)
        {
            throw new ArgumentException("That username already exists.");
        }

        _context.Employees.Add(employee);
        _context.SaveChanges();
    }

    public IReadOnlyList<Asset> GetAssignedAssets(string username)
    {
        return _assets.GetAll()
            .Where(asset => string.Equals(
                asset.EmployeeUsername,
                username.Trim(),
                StringComparison.OrdinalIgnoreCase))
            .OrderBy(asset => asset.Id)
            .ToList();
    }

    public void AssignAsset(int assetId, string username)
    {
        Employee employee = GetByUsername(username)
            ?? throw new ArgumentException("Employee not found.");

        Asset asset = _assets.GetById(assetId)
            ?? throw new ArgumentException("Asset not found.");

        if (!string.IsNullOrWhiteSpace(asset.EmployeeUsername)
            && !string.Equals(
                asset.EmployeeUsername,
                employee.Username,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Asset is already assigned. Unassign it first.");
        }

        asset.EmployeeUsername = employee.Username;

        _assets.Update(asset);
    }

    public void UnassignAsset(int assetId)
    {
        Asset asset = _assets.GetById(assetId)
            ?? throw new ArgumentException("Asset not found.");

        asset.EmployeeUsername = null;

        _assets.Update(asset);
    }

    private static void ValidateText(
        string value,
        int maximumLength,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{fieldName} must contain 1–{maximumLength} characters.");
        }
    }
}