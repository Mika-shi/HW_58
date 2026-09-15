using HW_58.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HW_58.Controllers;

public class HomeController : Controller
{
    private readonly InstagramContext _context;
    private readonly UserManager<User> _userManager;

    public HomeController(
        InstagramContext context,
        UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            ViewBag.IsAuthenticated = false;
            return View(new List<Post>());
        }

        ViewBag.IsAuthenticated = true;

        string? currentUserId = _userManager.GetUserId(User);

        if (currentUserId == null)
        {
            return View(new List<Post>());
        }

        List<string> followingIds = await _context.Follows
            .Where(follow => follow.FollowerId == currentUserId)
            .Select(follow => follow.FollowingId)
            .ToListAsync();

        List<Post> posts = await _context.Posts
            .Include(post => post.User)
            .Where(post => followingIds.Contains(post.UserId))
            .OrderByDescending(post => post.CreatedOn)
            .ToListAsync();

        return View(posts);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}