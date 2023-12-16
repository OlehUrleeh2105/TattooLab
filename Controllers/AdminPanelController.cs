using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TatooLab.Data;
using TatooLab.Filters;
using TatooLab.Models;
using TatooLab.Models.Views;

namespace TatooLab.Controllers;

[MasterAuthorization]
public class AdminPanelController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AdminPanelController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
        ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _webHostEnvironment = webHostEnvironment;
    }

    # region GET

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tattoos = await _dbContext.Tattoos.ToListAsync();
        return View(tattoos);
    }

    [HttpGet]
    public IActionResult AddTattoo()
    {
        return View();
    }

    [HttpGet]
    [AdminAuthorization]
    public async Task<IActionResult> ChangeUsersRole()
    {
        var users = await _userManager.Users.ToListAsync();
        var availableRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

        var viewModel = new List<UserRoleViewModel>();

        foreach (var user in users)
        {
            var userRoles = _userManager.GetRolesAsync(user).Result;
            var userViewModel = new UserRoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                AvailableRoles = availableRoles!,
                SelectedRole = userRoles.FirstOrDefault()
            };

            viewModel.Add(userViewModel);
        }

        return View(viewModel);
    }


    [HttpGet]
    public async Task<IActionResult> EditTattoo(int id)
    {
        var tattoo = await _dbContext.Tattoos.Include(t => t.TattooImages).FirstOrDefaultAsync(t => t.Id == id);

        if (tattoo == null)
        {
            return NotFound();
        }

        return View(tattoo);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteTattoo(int id)
    {
        var tattoo = await _dbContext.Tattoos.FirstOrDefaultAsync(t => t.Id == id);

        if (tattoo == null)
        {
            return NotFound();
        }

        var tattooImages = await _dbContext.TattooImages.Where(ti => ti.TattooId == id).ToListAsync();

        _dbContext.TattooImages.RemoveRange(tattooImages);
        _dbContext.Tattoos.Remove(tattoo);

        foreach (var tattooImage in tattooImages)
        {
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", tattooImage.ImageUrl!);

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        _dbContext.TattooImages.RemoveRange(tattooImages);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    # endregion

    # region POST

    [HttpPost]
    public async Task<IActionResult> AddTattoo(Tattoo model, List<IFormFile> Images)
    {
        if (ModelState.IsValid)
        {
            await _dbContext.Tattoos.AddAsync(model);
            await _dbContext.SaveChangesAsync();

            foreach (var image in Images)
            {
                if (image.Length > 0)
                {
                    var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", imageFileName);

                    await using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var tattooImage = new TattooImage
                    {
                        ImageUrl = imageFileName,
                        TattooId = model.Id
                    };

                    _dbContext.TattooImages.Add(tattooImage);
                }
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeRole(UserRoleViewModel? viewModel)
    {
        var user = await _userManager.FindByIdAsync(viewModel?.UserId!);

        if (user != null)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (result.Succeeded)
            {
                result = await _userManager.AddToRoleAsync(user, viewModel?.SelectedRole!);
                if (result.Succeeded) return RedirectToAction("Index");
            }
        }

        return RedirectToAction("ChangeUsersRole");
    }

    [HttpPost]
    public async Task<IActionResult> EditTattoo(Tattoo model, List<IFormFile> Images)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existingTattoo = await _dbContext.Tattoos.FirstOrDefaultAsync(t => t.Id == model.Id);

        if (existingTattoo == null)
        {
            return NotFound();
        }

        foreach (var image in Images)
        {
            if (image.Length > 0)
            {
                var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", imageFileName);

                await using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var tattooImage = new TattooImage
                {
                    ImageUrl = imageFileName,
                    TattooId = model.Id
                };

                _dbContext.TattooImages.Add(tattooImage);
            }
        }
        
        existingTattoo.Name = model.Name;
        existingTattoo.Price = model.Price;
        existingTattoo.Description = model.Description;

        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ActionName("DeleteImage")]
    public async Task<IActionResult> DeleteImagePost(string imageName)
    {
        var imageToDelete = await _dbContext.TattooImages
            .FirstOrDefaultAsync(image => image.ImageUrl == imageName);

        if (imageToDelete != null)
        {
            try
            {
                _dbContext.TattooImages.Remove(imageToDelete);
                await _dbContext.SaveChangesAsync();

                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", imageName);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                return RedirectToAction("EditTattoo", new { id = imageToDelete.TattooId });
            }
            catch
            {
                return NotFound();
            }
        }

        return NotFound();
    }

    
    # endregion
}