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

    public async Task<IActionResult> Details(int id)
    {
        Post? post = await _context.Posts
            .Include(post => post.User)
            .Include(post => post.Comments)
            .ThenInclude(comment => comment.User)
            .FirstOrDefaultAsync(post => post.Id == id);

        if (post == null)
        {
            return NotFound();
        }

        bool hasLiked = false;
        
        string? currentUserId = _userManager.GetUserId(User);

        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            if (currentUserId != null)
            {
                hasLiked = await _context.PostLikes
                    .AnyAsync(like =>
                        like.PostId == id &&
                        like.UserId == currentUserId);
            }
        }

        ViewBag.HasLiked = hasLiked;

        ViewBag.IsMyPost =
            currentUserId != null &&
            post.UserId == currentUserId;

        return View(post);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Like(int id)
    {
        string? currentUserId = _userManager.GetUserId(User);

        if (currentUserId == null)
        {
            return Json(new {success = false});
        }

        Post? post = await _context.Posts
            .FirstOrDefaultAsync(post => post.Id == id);

        if (post == null)
        {
            return Json(new {success = false});
        }

        bool alreadyLiked = await _context.PostLikes
            .AnyAsync(like =>
                like.PostId == id &&
                like.UserId == currentUserId);

        if (!alreadyLiked)
        {
            PostLike like = new PostLike
            {
                PostId = id,
                UserId = currentUserId,
                CreatedOn = DateTime.UtcNow
            };

            _context.PostLikes.Add(like);

            post.LikesCount++;

            await _context.SaveChangesAsync();
        }

        return Json(new {success = true, likesCount = post.LikesCount});
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Unlike(int id)
    {
        string? currentUserId = _userManager.GetUserId(User);

        if (currentUserId == null)
        {
            return Json(new { success = false });
        }

        Post? post = await _context.Posts
            .FirstOrDefaultAsync(post => post.Id == id);

        if (post == null)
        {
            return Json(new { success = false });
        }

        PostLike? like = await _context.PostLikes
            .FirstOrDefaultAsync(like =>
                like.PostId == id &&
                like.UserId == currentUserId);

        if (like != null)
        {
            _context.PostLikes.Remove(like);

            if (post.LikesCount > 0)
            {
                post.LikesCount--;
            }

            await _context.SaveChangesAsync();
        }

        return Json(new
        {
            success = true,
            likesCount = post.LikesCount
        });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AddComment(int postId, string text)
    {
        string? currentUserId = _userManager.GetUserId(User);

        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        Post? post = await _context.Posts
            .FirstOrDefaultAsync(post => post.Id == postId);

        if (post == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(text))
        {
            Comment comment = new Comment
            {
                Text = text.Trim(),
                CreatedOn = DateTime.UtcNow,
                PostId = postId,
                UserId = currentUserId
            };

            _context.Comments.Add(comment);

            post.CommentsCount++;

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Details", new { id = postId });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteAjax(int id)
    {
        string? currentUserId = _userManager.GetUserId(User);

        if (currentUserId == null)
        {
            return Json(new { success = false });
        }

        Post? post = await _context.Posts.FirstOrDefaultAsync(post => post.Id == id);

        if (post == null)
        {
            return Json(new { success = false });
        }

        if (post.UserId != currentUserId)
        {
            return Json(new { success = false });
        }

        _context.Posts.Remove(post);

        await _context.SaveChangesAsync();

        return Json(new
        {success = true, userId = currentUserId});
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> EditDescription(int id, string description)
    {
        string? currentUserId = _userManager.GetUserId(User);

        if (currentUserId == null)
        {
            return Json(new { success = false });
        }

        Post? post = await _context.Posts
            .FirstOrDefaultAsync(post => post.Id == id);

        if (post == null)
        {
            return Json(new { success = false });
        }

        if (post.UserId != currentUserId)
        {
            return Json(new { success = false });
        }

        post.Description = description;

        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            description = post.Description
        });
    }
}