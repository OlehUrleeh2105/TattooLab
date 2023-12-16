using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TatooLab.Interfaces;
using TatooLab.Models;
using TatooLab.Models.Views;

namespace TatooLab.Services;

public class UserService : IUserService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public UserService(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<bool> AuthenticateByEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            await _signInManager.SignInAsync(user, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            });
            return true;
        }
        return false;
    }

    public async Task<bool> Authenticate(LoginViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email!);

        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, model.Password!, true, false);

            if (result.Succeeded) return true;
        }

        return false;
    }

    public async Task<string> GetCurrentUserId(ClaimsPrincipal? user)
    {
        if (user == null)
        {
            return null!;
        }

        var identityUser = await _userManager.GetUserAsync(user);

        if (identityUser == null)
        {
            return null!;
        }

        return identityUser.Id;
    }

    public async Task<bool> Register(RegisterViewModel model)
    {
        string role = _userManager.Users.Any() ? "Customer" : "Admin";

        if (await CreateUser(model, role))
        {
            return true;
        }

        return false;
    }

    public async Task<bool> CreateUser(RegisterViewModel model, string role)
    {
        if (await CheckEmailExist(model.Email!))
        {
            return false;
        }

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            MiddleName = model.MiddleName
        };

        var result = await _userManager.CreateAsync(user, model.Password!);

        if (result.Succeeded)
        {
            result = await _userManager.AddToRoleAsync(user, role);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, true);
                return true;
            }
        }

        return false;
    }

    public async Task<bool> CheckEmailExist(string email)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            return true;
        }
        
        return false;
    }

    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }
}