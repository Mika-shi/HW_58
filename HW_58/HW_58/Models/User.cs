using HW_58.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace HW_58.Models;

public class User : IdentityUser
{
    public string AvatarPath { get; set; } = "";

    public string FullName { get; set; } = "";

    public string Bio { get; set; } = "";

    public Gender Gender { get; set; } = Gender.NotSpecified;

    public int PostsCount { get; set; } = 0;

    public int FollowersCount { get; set; } = 0;

    public int FollowingCount { get; set; } = 0;

    public List<Post> Posts { get; set; } = new List<Post>();

    public List<Follow> Followers { get; set; } = new List<Follow>();

    public List<Follow> Followings { get; set; } = new List<Follow>();

    public List<PostLike> Likes { get; set; } = new List<PostLike>();

    public List<Comment> Comments { get; set; } = new List<Comment>();
}