using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TatooLab.Models;

public class Tattoo : BaseEntity
{
    [Display(Name = "Title")]
    [Required(ErrorMessage = "Title is required")]
    public string? Name { get; set; }

    [Display(Name = "Price")]
    [Required(ErrorMessage = "Price is required")]
    [Range(0, float.MaxValue, ErrorMessage = "Price should be positive")]
    public float? Price { get; set; }

    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    [NotMapped] 
    public bool IsFavorite { get; set; }

    public List<TattooImage>? TattooImages { get; set; }
}