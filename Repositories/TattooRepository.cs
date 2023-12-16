    using Microsoft.EntityFrameworkCore;
    using TatooLab.Data;
    using TatooLab.Interfaces;
    using TatooLab.Models;

    namespace TatooLab.Repositories;

    public class TattooRepository : ITattooRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TattooRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tattoo> GetByIdAsync(int id) =>
            (await _dbContext.Tattoos.Include(t => t.TattooImages).FirstOrDefaultAsync(t => t.Id == id))!;


        public async Task<IReadOnlyList<Tattoo>> GetAllAsync() =>
            await _dbContext.Tattoos.Include(t => t.TattooImages).ToListAsync();
    }