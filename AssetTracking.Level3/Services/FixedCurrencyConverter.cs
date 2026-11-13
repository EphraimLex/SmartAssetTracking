namespace AssetTracking.Level3.Services;

// Converts USD amounts using fixed educational example rates.
// These rates are illustrative, not current market rates.
public class FixedCurrencyConverter : ICurrencyConverter
{
    // Each rate represents the local currency amount for 1 USD.
    private readonly Dictionary<string, decimal> _rates =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["USD"] = 1.00m,
            ["SEK"] = 10.00m,
            ["EUR"] = 0.90m,
            ["TRY"] = 35.00m
        };

    public decimal ConvertFromUsd(
        decimal amountUsd,
        string currencyCode)
    {
        if (amountUsd < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amountUsd),
                "The USD amount cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            throw new ArgumentException(
                "A currency code is required.",
                nameof(currencyCode));
        }

        if (!_rates.TryGetValue(currencyCode.Trim(), out decimal rate))
        {
            throw new ArgumentException(
                $"Unsupported currency code: {currencyCode}.",
                nameof(currencyCode));
        }

        return decimal.Round(
            amountUsd * rate,
            2,
            MidpointRounding.AwayFromZero);
    }
}