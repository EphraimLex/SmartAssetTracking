using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level3.Models;
using AssetTracking.Level3.Repositories;

namespace AssetTracking.Level3.ConsoleUI;

// Lets the user select an existing office instead of typing its name.
public class RegisteredOfficeLocationInput : IOfficeLocationInput
{
    private readonly ConsoleInput _input;
    private readonly IOfficeRepository _repository;
    private readonly OfficeDisplay _display;

    public RegisteredOfficeLocationInput(
        ConsoleInput input,
        IOfficeRepository repository,
        OfficeDisplay display)
    {
        _input = input;
        _repository = repository;
        _display = display;
    }

    public string ReadOfficeLocation()
    {
        IReadOnlyList<Office> offices = _repository.GetAll();

        if (offices.Count == 0)
        {
            throw new OfficeSelectionException(
                "No offices are available. Add or import offices first.");
        }

        _display.DisplayOffices(offices);

        while (true)
        {
            string input = _input.ReadRequiredText(
                "Select office ID (0 to cancel): ");

            if (input == "0")
            {
                throw new OfficeSelectionException(
                    "Office selection cancelled.");
            }

            if (!int.TryParse(input, out int id))
            {
                Console.WriteLine("Please enter a valid office ID.");
                continue;
            }

            Office? office = offices.FirstOrDefault(
                item => item.Id == id);

            if (office is null)
            {
                Console.WriteLine("No office has that ID.");
                continue;
            }

            // OfficeLocation currently stores only the office name.
            // Reject ambiguous names until we introduce an ID relationship.
            bool hasDuplicateName = offices.Any(
                item => item.Id != office.Id
                    && string.Equals(
                        item.OfficeName.Trim(),
                        office.OfficeName.Trim(),
                        StringComparison.OrdinalIgnoreCase));

            if (hasDuplicateName)
            {
                Console.WriteLine(
                    "This office name is not unique. " +
                    "Select an office with a unique name.");
                continue;
            }

            return office.OfficeName;
        }
    }
}