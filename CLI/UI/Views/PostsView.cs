namespace CLI.UI.Views;

using Entities;
using RepositoryContracts;

public class PostsView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;

    public PostsView(IPostRepository postRepo, IUserRepository userRepo, ICommentRepository commentRepo)
    {
        postRepository = postRepo;
        userRepository = userRepo;
        commentRepository = commentRepo;
    }

    public async Task CreatePostAsync()
    {
        Console.WriteLine("--- Create New Post ---");
        Console.Write("Enter User ID (Author): ");
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

        Console.Write("Enter Post Title: ");
        string title = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Error: Title cannot be empty.");
            PressEnterToContinue();
            return;
        }

        Console.Write("Enter Post Body: ");
        string body = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Error: Body cannot be empty.");
            PressEnterToContinue();
            return;
        }

        Post newPost = new Post
        {
            Title = title,
            Body = body,
            UserId = userId
        };

        Post createdPost = await postRepository.AddAsync(newPost);
        Console.WriteLine($"\n[SUCCESS] Post created successfully! Assigned Post ID: {createdPost.Id}");
        PressEnterToContinue();
    }

    public void ViewPostsOverview()
    {
        Console.WriteLine("--- Posts Overview ---");
        List<Post> posts = postRepository.GetMany().ToList();

        if (posts.Count == 0)
        {
            Console.WriteLine("No posts available.");
        }
        else
        {
            foreach (Post post in posts)
            {
                Console.WriteLine($"[Post ID: {post.Id}] Title: \"{post.Title}\" (Author User ID: {post.UserId})");
            }
        }

        PressEnterToContinue();
    }

    public async Task ViewSpecificPostAsync()
    {
        Console.WriteLine("--- View Specific Post ---");
        Console.Write("Enter Post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Error: Invalid Post ID format.");
            PressEnterToContinue();
            return;
        }

        Post post;
        try
        {
            post = await postRepository.GetSingleAsync(postId);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"Error: Post with ID {postId} does not exist.");
            PressEnterToContinue();
            return;
        }

        User? author = null;
        try
        {
            author = await userRepository.GetSingleAsync(post.UserId);
        }
        catch
        {
        }

        List<Comment> comments = commentRepository.GetMany().Where(c => c.PostId == postId).ToList();

        Console.WriteLine("\n==================================");
        Console.WriteLine($"POST #{post.Id}: {post.Title}");
        Console.WriteLine($"Author: {(author != null ? author.Username : "User #" + post.UserId)} (ID: {post.UserId})");
        Console.WriteLine("----------------------------------");
        Console.WriteLine(post.Body);
        Console.WriteLine("==================================");
        Console.WriteLine($"Comments ({comments.Count}):");

        if (comments.Count == 0)
        {
            Console.WriteLine("  (No comments on this post yet)");
        }
        else
        {
            foreach (Comment comment in comments)
            {
                Console.WriteLine($"  - [Comment #{comment.Id} by User #{comment.UserId}]: {comment.Body}");
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
