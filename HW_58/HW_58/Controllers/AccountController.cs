using HW_58.Models;
using HW_58.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HW_58.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IWebHostEnvironment _environment;

    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        /*if (!ModelState.IsValid)
        {
            return View(model);
        }*/

        string login = model.Login.Trim();
        string email = model.Email.Trim().ToLower();
        
        if (string.IsNullOrWhiteSpace(login))
        {
            ModelState.AddModelError("Login", "Enter login");
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError("Email", "Enter email");
        }

        User? userByLogin = await _userManager.FindByNameAsync(login);

        if (userByLogin != null)
        {
            ModelState.AddModelError("Login", "User with this login already exists");
        }

        User? userByEmail = await _userManager.FindByEmailAsync(email);

        if (userByEmail != null)
        {
            ModelState.AddModelError("Email", "User with this email already exists");
        }

        if (model.AvatarFile == null)
        {
            ModelState.AddModelError("AvatarFile", "Upload avatar");
            return View(model);
        }

        string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string fileExtension = Path.GetExtension(model.AvatarFile.FileName);
        string fileName = Guid.NewGuid() + fileExtension;
        string filePath = Path.Combine(uploadsFolder, fileName);

        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            await model.AvatarFile.CopyToAsync(stream);
        }
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        User user = new User
        {
            UserName = login,
            Email = email,
            AvatarPath = "/uploads/avatars/" + fileName,
            FullName = model.FullName.Trim(),
            Bio = model.Bio?.Trim() ?? "",
            PhoneNumber = model.PhoneNumber?.Trim() ?? "",
            Gender = model.Gender,
            PostsCount = 0,
            FollowersCount = 0,
            FollowingCount = 0
        };

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Index", "Home");
        }

        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string loginOrEmail = model.LoginOrEmail.Trim();

        User? user = await _userManager.FindByNameAsync(loginOrEmail);

        if (user == null)
        {
            user = await _userManager.FindByEmailAsync(loginOrEmail.ToLower());
        }

        if (user == null)
        {
            ModelState.AddModelError("", "Incorrect login/email or password");
            return View(model);
        }

        Microsoft.AspNetCore.Identity.SignInResult result =
            await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                false
            );

        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Incorrect login/email or password");

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}