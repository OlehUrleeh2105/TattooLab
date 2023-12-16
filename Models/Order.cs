using System.ComponentModel.DataAnnotations.Schema;

namespace TatooLab.Models;

public class Order : BaseEntity
{
    public string? ClientId { get; set; }
    public string? MasterId { get; set; }
    public int TattooId { get; set; }
    public DateTime BookingDateTime { get; set; }
    public bool Status { get; set; }

    [ForeignKey("ClientId")] public User? Client { get; set; }

    [ForeignKey("MasterId")] public User? Master { get; set; }

    [ForeignKey("TattooId")] public Tattoo? Tattoo { get; set; }
}