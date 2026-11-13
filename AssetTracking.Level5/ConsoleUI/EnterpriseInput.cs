using AssetTracking.Level1.ConsoleUI;

namespace AssetTracking.Level5.ConsoleUI;

// Adds numeric input without modifying the shared ConsoleInput class.
public static class EnterpriseInput
{
    public static int ReadNumber(
        ConsoleInput input,
        string prompt,
        int minimum,
        int maximum = int.MaxValue)
    {
        while (true)
        {
            string text = input.ReadRequiredText(prompt);

            if (int.TryParse(text, out int value)
                && value >= minimum
                && value <= maximum)
            {
                return value;
            }

            Console.WriteLine(
                $"Enter a whole number from {minimum} to {maximum}.");
        }
    }
}