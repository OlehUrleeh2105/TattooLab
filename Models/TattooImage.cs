namespace TatooLab.Models;

public class TattooImage : BaseEntity
{
    public string? ImageUrl { get; set; }

    public int TattooId { get; set; }
    public Tattoo? Tattoo { get; set; }
}