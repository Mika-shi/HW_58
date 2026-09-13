using HW_58.Models;
namespace HW_58.Models;

public class Follow
{
    public string FollowerId { get; set; } = "";

    public User? Follower { get; set; }

    public string FollowingId { get; set; } = "";

    public User? Following { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}