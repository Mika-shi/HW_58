using HW_58.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HW_58.Controllers;

public class ProfileController : Controller
{
    private readonly InstagramContext _context;
    private readonly UserManager<User> _userManager;

    public ProfileController(
        InstagramContext context,
        UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        string? currentUserId = _userManager.GetUserId(User);

        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        return RedirectToAction("Details", new { id = currentUserId });
    }

    public async  Task<IActionResult> Details(string id)
    {
        User? user = _context.Users
            .Include(user => user.Posts)
            .FirstOrDefault(user => user.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        string? currentUserId = _userManager.GetUserId(User);

        ViewBag.IsMyProfile = currentUserId == user.Id;
        
        bool isFollowing = false;

        if (currentUserId != null && currentUserId != user.Id)
        {
            isFollowing = await _context.Follows
                .AnyAsync(follow =>
                    follow.FollowerId == currentUserId &&
                    follow.FollowingId == user.Id);
        }

        ViewBag.IsFollowing = isFollowing;

        return View(user);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Follow(string id)
    {
        string? currentUserId = _userManager.GetUserId(User);

        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (currentUserId == id)
        {
            return RedirectToAction("Details", new { id = id });
        }

        User? currentUser = await _userManager.FindByIdAsync(currentUserId);
        User? targetUser = await _userManager.FindByIdAsync(id);

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (targetUser == null)
        {
            return NotFound();
        }

        bool alreadyFollowing = await _context.Follows
            .AnyAsync(follow =>
                follow.FollowerId == currentUserId &&
                follow.FollowingId == id);

        if (!alreadyFollowing)
        {
            Follow follow = new Follow
            {
                FollowerId = currentUserId,
                FollowingId = id,
                CreatedOn = DateTime.UtcNow
            };

            _context.Follows.Add(follow);

            currentUser.FollowingCount++;
            targetUser.FollowersCount++;

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Details", new { id = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Unfollow(string id)
    {
        string? currentUserId = _userManager.GetUserId(User);

        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        User? currentUser = await _userManager.FindByIdAsync(currentUserId);
        User? targetUser = await _userManager.FindByIdAsync(id);

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (targetUser == null)
        {
            return NotFound();
        }

        Follow? follow = await _context.Follows
            .FirstOrDefaultAsync(follow =>
                follow.FollowerId == currentUserId &&
                follow.FollowingId == id);

        if (follow != null)
        {
            _context.Follows.Remove(follow);

            if (currentUser.FollowingCount > 0)
            {
                currentUser.FollowingCount--;
            }

            if (targetUser.FollowersCount > 0)
            {
                targetUser.FollowersCount--;
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Details", new { id = id });
    }
}