using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TatooLab.Models;

public class User : IdentityUser
{
    [Required(ErrorMessage = "Name is required")]
    [Display(Name = "Name")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Surname is required")]
    [Display(Name = "Surname")]
    public string? LastName { get; set; }

    [Display(Name = "Father's name")] public string? MiddleName { get; set; }
    
    public List<Favorite>? Favorites { get; set; }

    public string GetFullname()
    {
        return $"{LastName} {FirstName} {MiddleName}";
    }
}