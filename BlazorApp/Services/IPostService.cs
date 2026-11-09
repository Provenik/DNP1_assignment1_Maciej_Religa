namespace BlazorApp.Services;

using ApiContracts;

public interface IPostService
{
    public Task<PostDto> AddPostAsync(CreatePostDto request);
    public Task UpdatePostAsync(int id, UpdatePostDto request);
    public Task<PostDto> GetSinglePostAsync(int id, bool includeComments = true);
    public Task<List<PostDto>> GetManyPostsAsync(string? titleContains = null, int? userId = null, string? userName = null);
    public Task DeletePostAsync(int id);
}
