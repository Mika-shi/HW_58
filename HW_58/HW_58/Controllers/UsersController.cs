using HW_58.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;


namespace HW_58.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly InstagramContext _context;
    private readonly UserManager<User> _userManager;

    public UsersController(InstagramContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Search(string? keyword)
    {
        List<User> users = new List<User>();
        
        string? currentUserId = _userManager.GetUserId(User);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            string searchText = keyword.Trim().ToLower();

            users = _context.Users.Where(user => user.Id != currentUserId && (
                                   user.UserName!.ToLower().Contains(searchText) ||
                                   user.Email!.ToLower().Contains(searchText) ||
                                   user.FullName.ToLower().Contains(searchText) ||
                                   user.Bio.ToLower().Contains(searchText)
                )).OrderBy(user => user.UserName).ToList();
        }

        ViewBag.Keyword = keyword;

        return View(users);
    }
}