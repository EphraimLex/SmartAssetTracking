using AssetTracking.Level1.ConsoleUI.Actions;
using AssetTracking.Level3.Models;
using AssetTracking.Level3.Repositories;

namespace AssetTracking.Level3.ConsoleUI.Actions;

// Retrieves company offices and displays them as a table.
public class DisplayOfficesAction : IMenuAction
{
    private readonly IOfficeRepository _repository;
    private readonly OfficeDisplay _officeDisplay;

    public DisplayOfficesAction(
        IOfficeRepository repository,
        OfficeDisplay officeDisplay)
    {
        _repository = repository;
        _officeDisplay = officeDisplay;
    }

    public string Key => "8";

    public string Description => "Display offices";

    public void Execute()
    {
        IReadOnlyList<Office> offices = _repository.GetAll();

        _officeDisplay.DisplayOffices(offices);
    }
}