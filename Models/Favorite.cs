namespace TatooLab.Models;

public class Favorite : BaseEntity
{
    public int TattooId { get; set; }
    public Tattoo? Tattoo { get; set; }
    public string? UserId { get; set; }
    public User? User { get; set; }
}