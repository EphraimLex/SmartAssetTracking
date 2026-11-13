using AssetTracking.Level1.Models;

namespace AssetTracking.Level1.ConsoleUI;

// Displays assets in a shared table layout with lifetime colors.
public class AssetDisplay : IAssetDisplay
{
    public void DisplayAssets(IReadOnlyList<Asset> assets)
    {
        DisplayTable(assets, groupByCategory: false);
    }

    public void DisplayByCategory(IReadOnlyList<Asset> assets)
    {
        DisplayTable(assets, groupByCategory: true);
    }

    // Shares formatting between the standard and grouped views.
    private static void DisplayTable(
        IReadOnlyList<Asset> assets,
        bool groupByCategory)
    {
        const string title =
            "================ COMPANY ASSETS ================";

        Console.WriteLine();
        Console.WriteLine(title);
        Console.WriteLine();

        if (assets.Count == 0)
        {
            Console.WriteLine("No assets found.");
            Console.WriteLine();
            Console.WriteLine(new string('=', title.Length));
            return;
        }

        DateTime today = DateTime.Today;

        // Calculate widths once so all category sections align.
        int idWidth = Math.Max(
            4, assets.Max(asset => asset.Id.ToString().Length) + 2);

        int brandWidth = Math.Max(
            9, assets.Max(asset => asset.Brand.Length) + 2);

        int modelWidth = Math.Max(
            15, assets.Max(asset => asset.ModelName.Length) + 2);

        const int dateWidth = 16;
        const int statusWidth = 6;

        string header =
            "ID".PadRight(idWidth) +
            "Brand".PadRight(brandWidth) +
            "Model".PadRight(modelWidth) +
            "Purchase Date".PadRight(dateWidth) +
            "Status";

        int tableWidth = Math.Max(
            title.Length,
            idWidth + brandWidth + modelWidth + dateWidth + statusWidth);

        string separator = new('-', tableWidth);

        if (groupByCategory)
        {
            var categories = assets
                .GroupBy(asset => asset switch
                {
                    ComputerAsset => "Computers",
                    MobileAsset => "Mobile Devices",
                    _ => "Other Assets"
                })
                .OrderBy(group => group.Key);

            bool firstCategory = true;

            foreach (var category in categories)
            {
                Console.WriteLine();
                Console.WriteLine(category.Key);
                Console.WriteLine(separator);

                // Show column headings once, as in the example.
                if (firstCategory)
                {
                    Console.WriteLine(header);
                    Console.WriteLine(separator);
                    firstCategory = false;
                }

                var sortedAssets = category
                    .OrderBy(asset => asset.PurchaseDate)
                    .ThenBy(asset => asset.Id);

                foreach (Asset asset in sortedAssets)
                {
                    DisplayRow(
                        asset, today,
                        idWidth, brandWidth, modelWidth, dateWidth);
                }

                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine(separator);
            Console.WriteLine(header);
            Console.WriteLine(separator);

            foreach (Asset asset in assets.OrderBy(asset => asset.Id))
            {
                DisplayRow(
                    asset, today,
                    idWidth, brandWidth, modelWidth, dateWidth);
            }

            Console.WriteLine();
        }

        Console.WriteLine(new string('=', tableWidth));

        // Display the legend only once, after the complete table.
        Console.WriteLine();
        Console.WriteLine(
            "RED: less than 3 months remaining or expired.");
        Console.WriteLine(
            "YELLOW: 3 to less than 6 months remaining.");
        Console.WriteLine(
            "NORMAL: 6 months or more remaining.");
    }

    // Writes one asset row and restores the original console color.
    private static void DisplayRow(
        Asset asset,
        DateTime today,
        int idWidth,
        int brandWidth,
        int modelWidth,
        int dateWidth)
    {
        DateTime endOfLife = asset.PurchaseDate.Date.AddYears(3);

        var (status, color) = GetLifetimeStatus(endOfLife, today);

        ConsoleColor originalColor = Console.ForegroundColor;

        try
        {
            if (color.HasValue)
            {
                Console.ForegroundColor = color.Value;
            }

            Console.WriteLine(
                asset.Id.ToString().PadRight(idWidth) +
                asset.Brand.PadRight(brandWidth) +
                asset.ModelName.PadRight(modelWidth) +
                asset.PurchaseDate.ToString("yyyy-MM-dd").PadRight(dateWidth) +
                status);
        }
        finally
        {
            Console.ForegroundColor = originalColor;
        }
    }

    // Uses the agreed colors, with the most urgent condition first.
    private static (string Status, ConsoleColor? Color) GetLifetimeStatus(
        DateTime endOfLife,
        DateTime today)
    {
        if (endOfLife < today.AddMonths(3))
        {
            return ("RED", ConsoleColor.Red);
        }

        if (endOfLife < today.AddMonths(6))
        {
            return ("YELLOW", ConsoleColor.Yellow);
        }

        return ("NORMAL", null);
    }
}