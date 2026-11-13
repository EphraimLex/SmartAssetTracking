using AssetTracking.Level1.Models;

namespace AssetTracking.Level1.ConsoleUI;

// Collects and validates input for creating and updating assets.
public class AssetInput
{
    private readonly ConsoleInput _input;
    private readonly IOfficeLocationInput _officeLocationInput;

    // Preserves free-text office input for Levels 1 and 2.
    public AssetInput(ConsoleInput input)
        : this(input, new TextOfficeLocationInput(input))
    {
    }

    // Allows Level 3 to supply selection from registered offices.
    public AssetInput(
        ConsoleInput input,
        IOfficeLocationInput officeLocationInput)
    {
        _input = input;
        _officeLocationInput = officeLocationInput;
    }

    public Asset ReadAsset()
    {
        Console.WriteLine();

        string assetType = ReadAssetType();

        string? computerType = assetType == "1"
            ? ReadComputerType()
            : null;

        string brand = _input.ReadRequiredText("Brand: ");
        string modelName = _input.ReadRequiredText("Model name: ");

        DateTime purchaseDate = _input.ReadDate(
            "Purchase date (yyyy-MM-dd): ");

        decimal purchasePriceUsd = _input.ReadPrice(
            "Purchase price (USD): ");

        // Uses the office input implementation supplied to this class.
        string officeLocation = _officeLocationInput.ReadOfficeLocation();

        string serialNumber = _input.ReadRequiredText(
            "Serial number: ");

        DateTime warrantyExpirationDate = ReadWarrantyDate(purchaseDate);

        string? employeeUsername = _input.ReadOptionalText(
            "Employee username (Enter to skip): ");

        if (computerType is not null)
        {
            return new ComputerAsset(
                computerType,
                brand,
                modelName,
                purchaseDate,
                purchasePriceUsd,
                officeLocation,
                serialNumber,
                warrantyExpirationDate,
                employeeUsername);
        }

        return new MobileAsset(
            brand,
            modelName,
            purchaseDate,
            purchasePriceUsd,
            officeLocation,
            serialNumber,
            warrantyExpirationDate,
            employeeUsername);
    }

    // Reads a valid asset identifier.
    // Reads an asset ID, or cancels the current action when 0 is entered.
    public int ReadId()
    {
        while (true)
        {
            string input = _input.ReadRequiredText(
                "Asset ID (0 to cancel): ");

            if (input == "0")
            {
                throw new ActionCancelledException("Action cancelled.");
            }

            if (int.TryParse(input, out int id) && id > 0)
            {
                return id;
            }

            Console.WriteLine(
                "Please enter a positive asset ID, or 0 to cancel.");
        }
    }

    // Updates one selected property of an existing asset.
    public void ReadChanges(Asset asset)
    {
        Console.WriteLine();
        Console.WriteLine($"Editing: {asset.Brand} {asset.ModelName}");
        Console.WriteLine();

        Console.WriteLine("1. Brand");
        Console.WriteLine("2. Model name");
        Console.WriteLine("3. Purchase date");
        Console.WriteLine("4. Purchase price (USD)");
        Console.WriteLine("5. Office location");
        Console.WriteLine("6. Serial number");
        Console.WriteLine("7. Warranty expiration date");
        Console.WriteLine("8. Employee username");

        if (asset is ComputerAsset)
        {
            Console.WriteLine("9. Asset type (Laptop / Desktop)");
        }

        while (true)
        {
            string choice = _input.ReadRequiredText(
                "Select the property to update: ");

            switch (choice)
            {
                case "1":
                    Console.WriteLine($"Current brand: {asset.Brand}");

                    asset.Brand = _input.ReadRequiredText("New brand: ");
                    return;

                case "2":
                    Console.WriteLine(
                        $"Current model: {asset.ModelName}");

                    asset.ModelName = _input.ReadRequiredText(
                        "New model name: ");
                    return;

                case "3":
                    Console.WriteLine(
                        $"Current purchase date: " +
                        $"{asset.PurchaseDate:yyyy-MM-dd}");

                    while (true)
                    {
                        DateTime date = _input.ReadDate(
                            "New purchase date (yyyy-MM-dd): ");

                        if (date <= asset.WarrantyExpirationDate)
                        {
                            asset.PurchaseDate = date;
                            return;
                        }

                        Console.WriteLine(
                            "Purchase date cannot be after warranty expiration.");
                    }

                case "4":
                    Console.WriteLine(
                        $"Current purchase price (USD): " +
                        $"{asset.PurchasePriceUsd:F2}");

                    asset.PurchasePriceUsd = _input.ReadPrice(
                        "New purchase price (USD): ");
                    return;

                case "5":
                    Console.WriteLine(
                        $"Current office: {asset.OfficeLocation}");

                    // Uses the same office input strategy as creation.
                    asset.OfficeLocation =
                        _officeLocationInput.ReadOfficeLocation();
                    return;

                case "6":
                    Console.WriteLine(
                        $"Current serial number: {asset.SerialNumber}");

                    asset.SerialNumber = _input.ReadRequiredText(
                        "New serial number: ");
                    return;

                case "7":
                    Console.WriteLine(
                        $"Current warranty expiration: " +
                        $"{asset.WarrantyExpirationDate:yyyy-MM-dd}");

                    asset.WarrantyExpirationDate = ReadWarrantyDate(
                        asset.PurchaseDate);
                    return;

                case "8":
                    Console.WriteLine(
                        $"Current employee: " +
                        $"{asset.EmployeeUsername ?? "(none)"}");

                    asset.EmployeeUsername = _input.ReadOptionalText(
                        "New employee username (Enter to clear): ");
                    return;

                case "9" when asset is ComputerAsset:
                    Console.WriteLine(
                        $"Current asset type: {asset.AssetType}");

                    asset.AssetType = ReadComputerType();
                    return;

                default:
                    Console.WriteLine(
                        asset is ComputerAsset
                            ? "Please select a number from 1 to 9."
                            : "Please select a number from 1 to 8.");
                    break;
            }
        }
    }

    private string ReadAssetType()
    {
        Console.WriteLine("1. Computer");
        Console.WriteLine("2. Mobile phone or tablet");

        while (true)
        {
            string choice = _input.ReadRequiredText(
                "Select asset type: ");

            if (choice is "1" or "2")
            {
                return choice;
            }

            Console.WriteLine("Please select 1 or 2.");
        }
    }

    private string ReadComputerType()
    {
        Console.WriteLine();
        Console.WriteLine("1. Laptop");
        Console.WriteLine("2. Desktop");

        while (true)
        {
            string choice = _input.ReadRequiredText(
                "Select computer type: ");

            if (choice == "1")
            {
                return "Laptop";
            }

            if (choice == "2")
            {
                return "Desktop";
            }

            Console.WriteLine("Please select 1 or 2.");
        }
    }

    // Shares warranty-date validation between creation and updating.
    private DateTime ReadWarrantyDate(DateTime purchaseDate)
    {
        while (true)
        {
            DateTime date = _input.ReadDate(
                "Warranty expiration date (yyyy-MM-dd): ");

            if (date >= purchaseDate)
            {
                return date;
            }

            Console.WriteLine(
                "Warranty expiration cannot be before the purchase date.");
        }
    }
}