namespace BlazorApp.Services;

using ApiContracts;

public interface IUserService
{
    public Task<UserDto> AddUserAsync(CreateUserDto request);
    public Task UpdateUserAsync(int id, UpdateUserDto request);
    public Task<UserDto> GetSingleUserAsync(int id);
    public Task<List<UserDto>> GetManyUsersAsync(string? usernameContains = null);
    public Task DeleteUserAsync(int id);
}
