namespace HW_58.Models;

public class Post
{
    public int Id { get; set; }

    public string ImagePath { get; set; } = "";

    public string Description { get; set; } = "";

    public int LikesCount { get; set; } = 0;

    public int CommentsCount { get; set; } = 0;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = "";

    public User? User { get; set; }

    public List<PostLike> Likes { get; set; } = new List<PostLike>();

    public List<Comment> Comments { get; set; } = new List<Comment>();
}