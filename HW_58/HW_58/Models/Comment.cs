using HW_58.Models;

namespace HW_58.Models;

public class Comment
{
    public int Id { get; set; }

    public string Text { get; set; } = "";

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public int PostId { get; set; }

    public Post? Post { get; set; }

    public string UserId { get; set; } = "";

    public User? User { get; set; }
}