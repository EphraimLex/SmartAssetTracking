using AssetTracking.Level1.ConsoleUI;
using AssetTracking.Level5.Models;
using AssetTracking.Level5.Services;

namespace AssetTracking.Level5.ConsoleUI;

// Collects the username for the demonstration login.
public class LoginScreen
{
    private readonly ConsoleInput _input;
    private readonly LoginService _login;

    public LoginScreen(
        ConsoleInput input,
        LoginService login)
    {
        _input = input;
        _login = login;
    }

    public Employee Show()
    {
        Console.WriteLine();
        Console.WriteLine("====== LEVEL 5 LOGIN ======");
        Console.WriteLine("Simulation: enter a registered username.");
        Console.WriteLine("First-run demo username: admin");
        Console.WriteLine();

        while (true)
        {
            string username = _input.ReadRequiredText("Username: ");

            if (_login.Login(username)
                && _login.CurrentUser is Employee user)
            {
                Console.WriteLine();
                Console.WriteLine(
                    $"Welcome, {user.FullName} ({user.Role}).");

                return user;
            }

            Console.WriteLine("Username not found. Please try again.");
        }
    }
}