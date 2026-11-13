using AssetTracking.Level3.Models;

namespace AssetTracking.Level3.Services;

// Matches an asset's office text to one registered office.
// Returns null when the match is missing or ambiguous.
public class OfficeResolver
{
    public Office? Resolve(
        string officeLocation,
        IReadOnlyList<Office> offices)
    {
        if (string.IsNullOrWhiteSpace(officeLocation))
        {
            return null;
        }

        string location = NormalizeName(officeLocation);

        var matches = offices
            .Where(office => string.Equals(
                NormalizeName(office.OfficeName),
                location,
                StringComparison.OrdinalIgnoreCase))
            .Take(2)
            .ToList();

        return matches.Count == 1 ? matches[0] : null;
    }

    // Treats "Sweden" and "Sweden Office" as the same office label.
    private static string NormalizeName(string value)
    {
        string name = value.Trim();
        const string suffix = " Office";

        return name.EndsWith(
            suffix,
            StringComparison.OrdinalIgnoreCase)
                ? name[..^suffix.Length].TrimEnd()
                : name;
    }
}