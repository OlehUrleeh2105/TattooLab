using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TatooLab.Data;
using TatooLab.Models;
using TatooLab.Models.Views;
using TatooLab.Repositories;
using TatooLab.Services;

namespace TatooLab.Controllers;

public class TattooController : Controller
{
    private readonly TattooRepository _tattooRepository;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserService _userService;

    public TattooController(ApplicationDbContext dbContext, SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userService = new UserService(signInManager, userManager);
        _tattooRepository = new TattooRepository(_dbContext);
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? page, string? searchString, string? sortOrder, bool? onlyFavorite)
    {
        ViewData["OnlyFavorite"] = onlyFavorite;
        ViewBag.Sort = sortOrder!;
        ViewBag.SearchString = searchString!;
        
        var pageNumber = page ?? 1;
        var pageSize = 9;
        
        var tattoosQuery = await _tattooRepository.GetAllAsync();

        var userId = await _userService.GetCurrentUserId(User);
        
        if (onlyFavorite.HasValue && onlyFavorite.Value)
        {
            if (userId != null!)
            {
                var favoriteTattooIds = await GetFavoriteTattooIds(userId);
                tattoosQuery = tattoosQuery.Where(t => favoriteTattooIds.Contains(t.Id)).ToList();
            }
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            tattoosQuery = tattoosQuery.Where(t => t.Name!.ToLower().Trim().Contains(searchString.ToLower().Trim()))
                .ToList();
        }

        switch (sortOrder)
        {
            case "name_desc":
                tattoosQuery = tattoosQuery.OrderByDescending(t => t.Name).ToList();
                break;
            case "Price":
                tattoosQuery = tattoosQuery.OrderBy(t => t.Price).ToList();
                break;
            case "price_desc":
                tattoosQuery = tattoosQuery.OrderByDescending(t => t.Price).ToList();
                break;
            default:
                tattoosQuery = tattoosQuery.OrderBy(t => t.Name).ToList();
                break;
        }

        var totalTattoosCount = tattoosQuery.Count();

        var tattoos = tattoosQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        if (userId != null)
        {
            foreach (var tattoo in tattoos)
            {
                var isFavorite = await IsTattooInFavorites(userId, tattoo.Id);
                tattoo.IsFavorite = isFavorite;
            }
        }

        var tattoosView = new TattoosView
        {
            Tattoos = tattoos,
            PaginationInfo = new PaginationInfo
            {
                TotalItems = totalTattoosCount,
                ItemsPerPage = pageSize,
                CurrentPage = pageNumber
            },
            UserId = userId
        };
        
        return View(tattoosView);
    }

    private async Task<List<int>> GetFavoriteTattooIds(string userId)
    {
        var favoriteTattooIds = await _dbContext.Favorites
            .Where(uf => uf.UserId == userId)
            .Select(uf => uf.TattooId)
            .ToListAsync();
        
        return favoriteTattooIds;
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var tattoo = await _tattooRepository.GetByIdAsync(id);
        
        var userId = await _userService.GetCurrentUserId(User);
        
        var favorite =
            await _dbContext.Favorites.FirstOrDefaultAsync(f =>
                f.TattooId == id && f.UserId == userId);
        
        if (tattoo == null!)
            return NotFound();
        
        return View(new DetailsView
        {
            Tattoo = tattoo,
            Favorite = favorite != null,
            UserId = userId
        });
    }

    [HttpPost]
    public async Task<IActionResult> AddToFavorite(string userId, int tattooId)
    {
        var favoriteExist =
            await _dbContext.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.TattooId == tattooId);
        if (favoriteExist != null)
        {
            _dbContext.Remove(favoriteExist);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        
        await _dbContext.AddAsync(new Favorite
        {
            UserId = userId,
            TattooId = tattooId
        });
        await _dbContext.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    private async Task<bool> IsTattooInFavorites(string? userId, int tattooId)
    {
        var favoriteExist = await _dbContext.Favorites
            .AnyAsync(f => f.UserId == userId && f.TattooId == tattooId);
        
        return favoriteExist;
    }
}