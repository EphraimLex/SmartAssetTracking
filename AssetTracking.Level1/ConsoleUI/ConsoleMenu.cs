using AssetTracking.Level1.ConsoleUI.Actions;

namespace AssetTracking.Level1.ConsoleUI;

// Displays available actions and executes the user's selection.
public class ConsoleMenu
{
    private readonly IReadOnlyList<IMenuAction> _actions;

    public ConsoleMenu(IEnumerable<IMenuAction> actions)
    {
        _actions = actions.ToList();
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Smart Asset Tracking");
            Console.WriteLine("====================");

            foreach (IMenuAction action in _actions)
            {
                Console.WriteLine($"{action.Key}. {action.Description}");
            }

            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            string? input = Console.ReadLine();

            if (input is null || input.Trim() == "0")
            {
                Console.WriteLine();
                Console.WriteLine("Goodbye!");
                return;
            }

            IMenuAction? selectedAction = _actions
                .FirstOrDefault(action => action.Key == input.Trim());

            if (selectedAction is null)
            {
                Console.WriteLine("Invalid option. Please try again.");
                continue;
            }

            try
            {
                selectedAction.Execute();
            }
            catch (ActionCancelledException exception)
            {
                Console.WriteLine();
                Console.WriteLine(exception.Message);
            }
            catch (OfficeSelectionException exception)
            {
                Console.WriteLine();
                Console.WriteLine(exception.Message);
            }
        }
    }
}