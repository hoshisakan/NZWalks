using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NZWalks.API.Models.Domain;
using NZWalks.API.Data;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Repositories;


namespace NZWalks.API.Repositories
{
    public class DifficultyRepository : IDifficultyRepository
    {
        private readonly NZWalksDbContext _nZWalksDbContext;

        public DifficultyRepository(NZWalksDbContext nZWalksDbContext)
        {
            _nZWalksDbContext = nZWalksDbContext;
        }

        public async Task<List<Difficulty>> GetAllAsync()
        {
            return await _nZWalksDbContext.Difficulties.ToListAsync();
        }

        public async Task<Difficulty?> GetByIdAsync(Guid id)
        {
            return await _nZWalksDbContext.Difficulties.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Difficulty> CreateAsync(Difficulty difficulty)
        {
            await _nZWalksDbContext.Difficulties.AddAsync(difficulty);

            await _nZWalksDbContext.SaveChangesAsync();

            return difficulty;
        }

        public async Task<Difficulty?> UpdateAsync(Guid id, Difficulty difficulty)
        {
            var existingDifficulty = await _nZWalksDbContext.Difficulties.FirstOrDefaultAsync(x => x.Id == id);

            if (existingDifficulty == null)
            {
                return null;
            }

            existingDifficulty.Name = difficulty.Name;

            await _nZWalksDbContext.SaveChangesAsync();

            return existingDifficulty;
        }

        public async Task<Difficulty> DeleteAsync(Guid id)
        {
            var difficulty = await _nZWalksDbContext.Difficulties.FirstOrDefaultAsync(x => x.Id == id);

            if (difficulty == null)
            {
                return null;
            }

            _nZWalksDbContext.Difficulties.Remove(difficulty);

            await _nZWalksDbContext.SaveChangesAsync();

            return difficulty;
        }
    }
}