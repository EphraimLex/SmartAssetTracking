using AssetTracking.Level3.Models;

namespace AssetTracking.Level3.ConsoleUI;

// Displays company offices as an aligned table.
public class OfficeDisplay
{
    public void DisplayOffices(IReadOnlyList<Office> offices)
    {
        Console.WriteLine();
        Console.WriteLine("================ OFFICE LIST ================");
        Console.WriteLine();

        if (offices.Count == 0)
        {
            Console.WriteLine("No offices found.");
            return;
        }

        // Adjusts column widths to fit headings and values.
        int idWidth = Math.Max(
            "ID".Length,
            offices.Max(office => office.Id.ToString().Length));

        int nameWidth = Math.Max(
            "Office".Length,
            offices.Max(office => office.OfficeName.Length));

        int countryWidth = Math.Max(
            "Country".Length,
            offices.Max(office => office.Country.Length));

        int currencyWidth = Math.Max(
            "Currency".Length,
            offices.Max(office => office.CurrencyCode.Length));

        string header =
            $"{"ID".PadRight(idWidth)} | " +
            $"{"Office".PadRight(nameWidth)} | " +
            $"{"Country".PadRight(countryWidth)} | " +
            $"{"Currency".PadRight(currencyWidth)}";

        Console.WriteLine(header);
        Console.WriteLine(new string('-', header.Length));

        foreach (Office office in offices)
        {
            Console.WriteLine(
                $"{office.Id.ToString().PadRight(idWidth)} | " +
                $"{office.OfficeName.PadRight(nameWidth)} | " +
                $"{office.Country.PadRight(countryWidth)} | " +
                $"{office.CurrencyCode.PadRight(currencyWidth)}");
        }

        Console.WriteLine();
    }
}