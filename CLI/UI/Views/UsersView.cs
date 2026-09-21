namespace CLI.UI.Views;

using Entities;
using RepositoryContracts;

public class UsersView
{
    private readonly IUserRepository userRepository;

    public UsersView(IUserRepository userRepo)
    {
        userRepository = userRepo;
    }

    public async Task CreateUserAsync()
    {
        Console.WriteLine("--- Create New User ---");
        Console.Write("Enter Username: ");
        string username = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Error: Username cannot be empty.");
            PressEnterToContinue();
            return;
        }

        bool exists = userRepository.GetMany().Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (exists)
        {
            Console.WriteLine($"Error: Username '{username}' is already taken.");
            PressEnterToContinue();
            return;
        }

        Console.Write("Enter Password: ");
        string password = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Error: Password cannot be empty.");
            PressEnterToContinue();
            return;
        }

        User newUser = new User
        {
            Username = username,
            Password = password
        };

        User createdUser = await userRepository.AddAsync(newUser);
        Console.WriteLine($"\n[SUCCESS] User created successfully! Assigned User ID: {createdUser.Id}");
        PressEnterToContinue();
    }

    public void ListUsers()
    {
        Console.WriteLine("--- Registered Users List ---");
        List<User> users = userRepository.GetMany().ToList();

        if (users.Count == 0)
        {
            Console.WriteLine("No users registered yet.");
        }
        else
        {
            foreach (User user in users)
            {
                Console.WriteLine($"[User ID: {user.Id}] Username: {user.Username}");
            }
        }

        PressEnterToContinue();
    }

    private void PressEnterToContinue()
    {
        Console.WriteLine("\nPress Enter to return to the main menu...");
        Console.ReadLine();
    }
}
