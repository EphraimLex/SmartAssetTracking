using System.Globalization;

namespace AssetTracking.Level1.ConsoleUI;

// Reads console input and repeats the prompt until the value is valid.
public class ConsoleInput
{
    public string ReadRequiredText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string value = ReadLine().Trim();

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            Console.WriteLine("Please enter a value.");
        }
    }

    public string? ReadOptionalText(string prompt)
    {
        Console.Write(prompt);
        string value = ReadLine().Trim();

        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);

            if (DateTime.TryParseExact(
                ReadLine().Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date))
            {
                return date;
            }

            Console.WriteLine("Enter a valid date using yyyy-MM-dd.");
        }
    }

    public decimal ReadPrice(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);

            if (decimal.TryParse(
                ReadLine().Trim(),
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out decimal price)
                && price >= 0)
            {
                return price;
            }

            Console.WriteLine(
                "Enter a non-negative price using a decimal point, e.g. 1250.50.");
        }
    }

    private static string ReadLine()
    {
        return Console.ReadLine()
            ?? throw new OperationCanceledException("Console input ended.");
    }
}