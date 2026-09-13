namespace HW_58.Models;

public class PostLike
{
    public int PostId { get; set; }

    public Post? Post { get; set; }

    public string UserId { get; set; } = "";

    public User? User { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}