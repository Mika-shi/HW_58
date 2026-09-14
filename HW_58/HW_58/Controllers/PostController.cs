using HW_58.Models;
using HW_58.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HW_58.Controllers;
public class PostController : Controller
{
    private readonly InstagramContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _environment;

    public PostController(
        InstagramContext context,
        UserManager<User> userManager,
        IWebHostEnvironment environment)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
    }

    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(CreatePostViewModel model)
    {
        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        User? user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        string fileExtension = Path.GetExtension(model.ImageFile!.FileName).ToLower();

        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        if (!allowedExtensions.Contains(fileExtension))
        {
            ModelState.AddModelError("ImageFile", "Only JPG, JPEG, PNG or WEBP images are allowed.");
            return View(model);
        }

        string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "posts");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string fileName = Guid.NewGuid() + fileExtension;
        string filePath = Path.Combine(uploadsFolder, fileName);

        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            await model.ImageFile.CopyToAsync(stream);
        }

        Post post = new Post
        {
            ImagePath = "/uploads/posts/" + fileName,
            Description = model.Description.Trim(),
            LikesCount = 0,
            CommentsCount = 0,
            CreatedOn = DateTime.UtcNow,
            UserId = userId
        };

        _context.Posts.Add(post);

        user.PostsCount++;

        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = post.Id });
    }

    public IActionResult Details(int id)
    {
        Post? post = _context.Posts
            .Include(post => post.User)
            .FirstOrDefault(post => post.Id == id);

        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }
}