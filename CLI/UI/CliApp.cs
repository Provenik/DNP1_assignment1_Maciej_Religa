namespace CLI.UI;

using CLI.UI.Views;
using RepositoryContracts;

public class CliApp
{
    private readonly UsersView usersView;
    private readonly PostsView postsView;
    private readonly CommentsView commentsView;

    public CliApp(IUserRepository userRepo, IPostRepository postRepo, ICommentRepository commentRepo)
    {
        usersView = new UsersView(userRepo);
        postsView = new PostsView(postRepo, userRepo, commentRepo);
        commentsView = new CommentsView(commentRepo, postRepo, userRepo);
    }

    public async Task StartAsync()
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("==================================");
            Console.WriteLine("          FORUM CLI APP           ");
            Console.WriteLine("==================================");
            Console.WriteLine("1. Create New User");
            Console.WriteLine("2. Create New Post");
            Console.WriteLine("3. Add Comment to Post");
            Console.WriteLine("4. View Posts Overview");
            Console.WriteLine("5. View Specific Post");
            Console.WriteLine("6. List All Users");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            string? choice = Console.ReadLine()?.Trim();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    await usersView.CreateUserAsync();
                    break;
                case "2":
                    await postsView.CreatePostAsync();
                    break;
                case "3":
                    await commentsView.AddCommentAsync();
                    break;
                case "4":
                    postsView.ViewPostsOverview();
                    break;
                case "5":
                    await postsView.ViewSpecificPostAsync();
                    break;
                case "6":
                    usersView.ListUsers();
                    break;
                case "0":
                    running = false;
                    Console.WriteLine("Exiting CLI application. Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Press Enter to try again...");
                    Console.ReadLine();
                    break;
            }
        }
    }
}
