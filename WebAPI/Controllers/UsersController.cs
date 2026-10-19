namespace WebAPI.Controllers;

using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        try
        {
            await VerifyUserNameIsAvailableAsync(request.UserName);

            User user = new()
            {
                Username = request.UserName,
                Password = request.Password
            };

            User created = await userRepo.AddAsync(user);
            UserDto dto = new()
            {
                Id = created.Id,
                UserName = created.Username
            };

            return Created($"/Users/{dto.Id}", dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto request)
    {
        try
        {
            User existing = await userRepo.GetSingleAsync(id);
            existing.Username = request.UserName;
            existing.Password = request.Password;

            await userRepo.UpdateAsync(existing);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetSingleUser([FromRoute] int id)
    {
        try
        {
            User user = await userRepo.GetSingleAsync(id);
            UserDto dto = new()
            {
                Id = user.Id,
                UserName = user.Username
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
    public ActionResult<IEnumerable<UserDto>> GetManyUsers([FromQuery] string? usernameContains = null)
    {
        try
        {
            IQueryable<User> users = userRepo.GetMany();

            if (!string.IsNullOrWhiteSpace(usernameContains))
            {
                users = users.Where(u => u.Username.Contains(usernameContains, StringComparison.OrdinalIgnoreCase));
            }

            List<UserDto> dtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.Username
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
    public async Task<ActionResult> DeleteUser([FromRoute] int id)
    {
        try
        {
            await userRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    private Task VerifyUserNameIsAvailableAsync(string userName)
    {
        bool exists = userRepo.GetMany().Any(u => u.Username.Equals(userName, StringComparison.OrdinalIgnoreCase));
        if (exists)
        {
            throw new InvalidOperationException($"Username '{userName}' is already taken.");
        }

        return Task.CompletedTask;
    }
}
