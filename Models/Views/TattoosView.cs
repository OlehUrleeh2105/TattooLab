namespace TatooLab.Models.Views;

public class TattoosView
{
    public PaginationInfo? PaginationInfo { get; set; }
    public List<Tattoo>? Tattoos { get; set; }
    public string? UserId { get; set; }
}