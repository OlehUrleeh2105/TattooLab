using System.ComponentModel.DataAnnotations;

namespace TatooLab.Models.Views;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [Display(Name = "Name")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Surname is required")]
    [Display(Name = "Surname")]
    public string? LastName { get; set; }

    [Display(Name = "Father's name")] public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Password confirmation")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string? ConfirmPassword { get; set; }
    
}