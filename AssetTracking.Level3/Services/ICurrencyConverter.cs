namespace AssetTracking.Level3.Services;

// Converts USD amounts into a supported target currency.
public interface ICurrencyConverter
{
    decimal ConvertFromUsd(decimal amountUsd, string currencyCode);
}