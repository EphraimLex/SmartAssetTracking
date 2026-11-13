using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level3.Models;

namespace AssetTracking.Level3.ConsoleUI;

// Collects and validates office details through the console.
public class OfficeInput
{
    private readonly ConsoleInput _consoleInput;

    public OfficeInput(ConsoleInput consoleInput)
    {
        _consoleInput = consoleInput;
    }

    public Office ReadOffice()
    {
        Console.WriteLine();
        Console.WriteLine("Add Office");
        Console.WriteLine("==========");

        string officeName = ReadTextWithinLimit("Office name: ", 100);
        string country = ReadTextWithinLimit("Country: ", 100);
        string currencyCode = ReadCurrencyCode();

        return new Office(officeName, country, currencyCode);
    }

    private string ReadTextWithinLimit(string prompt, int maxLength)
    {
        while (true)
        {
            string value = _consoleInput.ReadRequiredText(prompt);

            if (value.Length <= maxLength)
            {
                return value;
            }

            Console.WriteLine(
                $"Please enter no more than {maxLength} characters.");
        }
    }

    private string ReadCurrencyCode()
    {
        while (true)
        {
            string currencyCode = _consoleInput
                .ReadRequiredText("Currency (SEK, USD, EUR, TRY): ")
                .ToUpperInvariant();

            // Initially supports the currencies required by the project.
            if (currencyCode is "SEK" or "USD" or "EUR" or "TRY")
            {
                return currencyCode;
            }

            Console.WriteLine(
                "Please enter SEK, USD, EUR, or TRY.");
        }
    }
}