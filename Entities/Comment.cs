namespace Entities;

public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int PostId { get; set; }
    
    public User User { get; set; } = null!;
    public Post Post { get; set; } = null!;
    public Comment() {}
}
