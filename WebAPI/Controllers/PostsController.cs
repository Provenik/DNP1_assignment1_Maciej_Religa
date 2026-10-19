namespace WebAPI.Controllers;

using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;
    private readonly ICommentRepository commentRepo;

    public PostsController(IPostRepository postRepo, IUserRepository userRepo, ICommentRepository commentRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
        this.commentRepo = commentRepo;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost([FromBody] CreatePostDto request)
    {
        try
        {
            User author = await userRepo.GetSingleAsync(request.UserId);

            Post post = new()
            {
                Title = request.Title,
                Body = request.Body,
                UserId = request.UserId
            };

            Post created = await postRepo.AddAsync(post);
            PostDto dto = new()
            {
                Id = created.Id,
                Title = created.Title,
                Body = created.Body,
                UserId = created.UserId,
                AuthorUserName = author.Username
            };

            return Created($"/Posts/{dto.Id}", dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdatePost([FromRoute] int id, [FromBody] UpdatePostDto request)
    {
        try
        {
            Post existing = await postRepo.GetSingleAsync(id);
            existing.Title = request.Title;
            existing.Body = request.Body;

            await postRepo.UpdateAsync(existing);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetSinglePost([FromRoute] int id, [FromQuery] bool includeComments = true)
    {
        try
        {
            Post post = await postRepo.GetSingleAsync(id);
            User? author = userRepo.GetMany().SingleOrDefault(u => u.Id == post.UserId);

            PostDto dto = new()
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId,
                AuthorUserName = author?.Username
            };

            if (includeComments)
            {
                List<User> allUsers = userRepo.GetMany().ToList();
                dto.Comments = commentRepo.GetMany()
                    .Where(c => c.PostId == post.Id)
                    .ToList()
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Body = c.Body,
                        UserId = c.UserId,
                        PostId = c.PostId,
                        AuthorUserName = allUsers.SingleOrDefault(u => u.Id == c.UserId)?.Username
                    })
                    .ToList();
            }

            return Ok(dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetManyPosts(
        [FromQuery] string? titleContains = null,
        [FromQuery] int? userId = null,
        [FromQuery] string? userName = null)
    {
        try
        {
            IQueryable<Post> posts = postRepo.GetMany();
            List<User> allUsers = userRepo.GetMany().ToList();

            if (!string.IsNullOrWhiteSpace(titleContains))
            {
                posts = posts.Where(p => p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));
            }

            if (userId.HasValue)
            {
                posts = posts.Where(p => p.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                List<int> matchingUserIds = allUsers
                    .Where(u => u.Username.Contains(userName, StringComparison.OrdinalIgnoreCase))
                    .Select(u => u.Id)
                    .ToList();

                posts = posts.Where(p => matchingUserIds.Contains(p.UserId));
            }

            List<PostDto> dtos = posts.ToList().Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                UserId = p.UserId,
                AuthorUserName = allUsers.SingleOrDefault(u => u.Id == p.UserId)?.Username
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePost([FromRoute] int id)
    {
        try
        {
            await postRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}
