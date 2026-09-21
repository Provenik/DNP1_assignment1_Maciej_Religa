namespace CLI.UI.Views;

using Entities;
using RepositoryContracts;

public class CommentsView
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public CommentsView(ICommentRepository commentRepo, IPostRepository postRepo, IUserRepository userRepo)
    {
        commentRepository = commentRepo;
        postRepository = postRepo;
        userRepository = userRepo;
    }

    public async Task AddCommentAsync()
    {
        Console.WriteLine("--- Add Comment to Post ---");
        Console.Write("Enter Post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Error: Invalid Post ID format.");
            PressEnterToContinue();
            return;
        }

        try
        {
            await postRepository.GetSingleAsync(postId);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"Error: Post with ID {postId} does not exist.");
            PressEnterToContinue();
            return;
        }

        Console.Write("Enter User ID (Author of Comment): ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Error: Invalid User ID format.");
            PressEnterToContinue();
            return;
        }

        try
        {
            await userRepository.GetSingleAsync(userId);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"Error: User with ID {userId} does not exist.");
            PressEnterToContinue();
            return;
        }

        Console.Write("Enter Comment Body: ");
        string body = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Error: Comment body cannot be empty.");
            PressEnterToContinue();
            return;
        }

        Comment newComment = new Comment
        {
            Body = body,
            UserId = userId,
            PostId = postId
        };

        Comment createdComment = await commentRepository.AddAsync(newComment);
        Console.WriteLine($"\n[SUCCESS] Comment added successfully! Assigned Comment ID: {createdComment.Id}");
        PressEnterToContinue();
    }

    private void PressEnterToContinue()
    {
        Console.WriteLine("\nPress Enter to return to the main menu...");
        Console.ReadLine();
    }
}
