using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TatooLab.Models;
using TatooLab.Models.Views;
using TatooLab.Services;

namespace TatooLab.Controllers;

public class AccountController : Controller
{
    private readonly UserService _userService;
    private readonly SignInManager<User> _signInManager;

    public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _userService = new UserService(signInManager, userManager);
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            if(await _userService.Authenticate(model)) return RedirectToAction("Index", "Tattoo");

            ModelState.AddModelError(string.Empty, "Login error.");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            if(await _userService.Register(model))
                return RedirectToAction("Index", "Tattoo");
        }
        ModelState.AddModelError(string.Empty, "User registration failed.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _userService.Logout();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

}