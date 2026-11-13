using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level3.Models;
using AssetTracking.Level3.Repositories;

namespace AssetTracking.Level3.ConsoleUI.Actions;

// Collects office details and saves the new office.
public class AddOfficeAction : IMenuAction
{
    private readonly OfficeInput _officeInput;
    private readonly IOfficeRepository _repository;

    public AddOfficeAction(
        OfficeInput officeInput,
        IOfficeRepository repository)
    {
        _officeInput = officeInput;
        _repository = repository;
    }

    public string Key => "7";

    public string Description => "Add office";

    public void Execute()
    {
        Office office = _officeInput.ReadOffice();

        _repository.Add(office);

        Console.WriteLine();
        Console.WriteLine(
            $"Office added successfully. ID: {office.Id}");
    }
}