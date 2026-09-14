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

    public IActionResult Details(string id)
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

        return View(user);
    }
}