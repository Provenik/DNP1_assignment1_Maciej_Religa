namespace WebAPI.Controllers;

using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public AuthController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    [HttpPost("login")]
    public ActionResult<UserDto> Login([FromBody] LoginRequest request)
    {
        User? user = userRepo.GetMany().SingleOrDefault(u => u.Username == request.UserName);
        if (user is null)
        {
            return Unauthorized("User not found");
        }

        if (user.Password != request.Password)
        {
            return Unauthorized("Incorrect password");
        }

        UserDto dto = new()
        {
            Id = user.Id,
            UserName = user.Username
        };

        return Ok(dto);
    }
}
