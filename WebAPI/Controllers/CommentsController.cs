namespace WebAPI.Controllers;

using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public CommentsController(ICommentRepository commentRepo, IPostRepository postRepo, IUserRepository userRepo)
    {
        this.commentRepo = commentRepo;
        this.postRepo = postRepo;
        this.userRepo = userRepo;
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment([FromBody] CreateCommentDto request)
    {
        try
        {
            await postRepo.GetSingleAsync(request.PostId);
            User author = await userRepo.GetSingleAsync(request.UserId);

            Comment comment = new()
            {
                Body = request.Body,
                UserId = request.UserId,
                PostId = request.PostId
            };

            Comment created = await commentRepo.AddAsync(comment);
            CommentDto dto = new()
            {
                Id = created.Id,
                Body = created.Body,
                UserId = created.UserId,
                PostId = created.PostId,
                AuthorUserName = author.Username
            };

            return Created($"/Comments/{dto.Id}", dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentDto request)
    {
        try
        {
            Comment existing = await commentRepo.GetSingleAsync(id);
            existing.Body = request.Body;

            await commentRepo.UpdateAsync(existing);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetSingleComment([FromRoute] int id)
    {
        try
        {
            Comment comment = await commentRepo.GetSingleAsync(id);
            User? author = userRepo.GetMany().SingleOrDefault(u => u.Id == comment.UserId);

            CommentDto dto = new()
            {
                Id = comment.Id,
                Body = comment.Body,
                UserId = comment.UserId,
                PostId = comment.PostId,
                AuthorUserName = author?.Username
            };

            return Ok(dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetManyComments(
        [FromQuery] int? userId = null,
        [FromQuery] string? userName = null,
        [FromQuery] int? postId = null)
    {
        try
        {
            IQueryable<Comment> comments = commentRepo.GetMany();
            List<User> allUsers = userRepo.GetMany().ToList();

            if (userId.HasValue)
            {
                comments = comments.Where(c => c.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                List<int> matchingUserIds = allUsers
                    .Where(u => u.Username.Contains(userName, StringComparison.OrdinalIgnoreCase))
                    .Select(u => u.Id)
                    .ToList();

                comments = comments.Where(c => matchingUserIds.Contains(c.UserId));
            }

            if (postId.HasValue)
            {
                comments = comments.Where(c => c.PostId == postId.Value);
            }

            List<CommentDto> dtos = comments.ToList().Select(c => new CommentDto
            {
                Id = c.Id,
                Body = c.Body,
                UserId = c.UserId,
                PostId = c.PostId,
                AuthorUserName = allUsers.SingleOrDefault(u => u.Id == c.UserId)?.Username
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
    public async Task<ActionResult> DeleteComment([FromRoute] int id)
    {
        try
        {
            await commentRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}
