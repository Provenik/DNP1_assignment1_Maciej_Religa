namespace BlazorApp.Services;

using ApiContracts;

public interface ICommentService
{
    public Task<CommentDto> AddCommentAsync(CreateCommentDto request);
    public Task UpdateCommentAsync(int id, UpdateCommentDto request);
    public Task<CommentDto> GetSingleCommentAsync(int id);
    public Task<List<CommentDto>> GetManyCommentsAsync(int? userId = null, string? userName = null, int? postId = null);
    public Task DeleteCommentAsync(int id);
}
