using Microsoft.EntityFrameworkCore;
using NZWalks.Domain.Entities;
using NZWalks.Domain.Repositories;
using NZWalks.Infrastructure.Data;

namespace NZWalks.Infrastructure.Repositories
{
    public class DifficultyRepository : IDifficultyRepository
    {
        private readonly NZWalksDbContext _context;

        public DifficultyRepository(NZWalksDbContext context)
        {
            _context = context;
        }

        public async Task<List<Difficulty>> GetAllAsync()
        {
            return await _context.Difficulties.ToListAsync();
        }

        public async Task<Difficulty?> GetByIdAsync(Guid id)
        {
            return await _context.Difficulties.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Difficulty> CreateAsync(Difficulty difficulty)
        {
            await _context.Difficulties.AddAsync(difficulty);
            await _context.SaveChangesAsync();
            return difficulty;
        }

        public async Task<Difficulty?> UpdateAsync(Guid id, Difficulty difficulty)
        {
            var existing = await _context.Difficulties.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return null;

            existing.Name = difficulty.Name;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<Difficulty?> DeleteAsync(Guid id)
        {
            var difficulty = await _context.Difficulties.FirstOrDefaultAsync(x => x.Id == id);
            if (difficulty == null) return null;

            _context.Difficulties.Remove(difficulty);
            await _context.SaveChangesAsync();
            return difficulty;
        }
    }
}
