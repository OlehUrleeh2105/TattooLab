using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TatooLab.Models;

namespace TatooLab.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tattoo> Tattoos { get; set; }
    public DbSet<TattooImage> TattooImages { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
}