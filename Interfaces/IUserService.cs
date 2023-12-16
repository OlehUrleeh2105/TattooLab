using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TatooLab.Models;
using TatooLab.Models.Views;

namespace TatooLab.Interfaces;

public interface IUserService
{
    Task<bool> AuthenticateByEmail(string email);
    Task<bool> Authenticate(LoginViewModel model);
    Task<string> GetCurrentUserId(ClaimsPrincipal user);
    Task<bool> Register(RegisterViewModel model);
    Task<bool> CreateUser(RegisterViewModel model, string role);
    Task<bool> CheckEmailExist(string email);
    Task Logout();
}