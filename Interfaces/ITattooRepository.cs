using TatooLab.Models;

namespace TatooLab.Interfaces;

public interface ITattooRepository
{
    Task<Tattoo> GetByIdAsync(int id);
    Task<IReadOnlyList<Tattoo>> GetAllAsync();
}