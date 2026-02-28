using Microsoft.EntityFrameworkCore;
using NZWalks.Domain.Entities;
using NZWalks.Domain.Repositories;
using NZWalks.Infrastructure.Data;

namespace NZWalks.Infrastructure.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext _context;

        public WalkRepository(NZWalksDbContext context)
        {
            _context = context;
        }

        public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            var walks = _context.Walks.Include(x => x.Difficulty).Include(x => x.Region).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                else if (filterOn.Equals("Description", StringComparison.OrdinalIgnoreCase))
                    walks = walks.Where(x => x.Description.Contains(filterQuery));
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                walks = sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase) ? (isAscending ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name))
                    : sortBy.Equals("LengthInKm", StringComparison.OrdinalIgnoreCase) ? (isAscending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm))
                    : sortBy.Equals("Region", StringComparison.OrdinalIgnoreCase) ? (isAscending ? walks.OrderBy(x => x.Region.Name) : walks.OrderByDescending(x => x.Region.Name))
                    : sortBy.Equals("Difficulty", StringComparison.OrdinalIgnoreCase) ? (isAscending ? walks.OrderBy(x => x.Difficulty.Name) : walks.OrderByDescending(x => x.Difficulty.Name))
                    : walks;
            }

            var skipNumber = (pageNumber - 1) * pageSize;
            return await walks.Skip(skipNumber).Take(pageSize).ToListAsync();
        }

        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            return await _context.Walks.Include(x => x.Difficulty).Include(x => x.Region).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Walk> CreateAsync(Walk walk)
        {
            await _context.Walks.AddAsync(walk);
            await _context.SaveChangesAsync();
            return walk;
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            var existing = await _context.Walks.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return null;

            existing.Name = walk.Name;
            existing.Description = walk.Description;
            existing.LengthInKm = walk.LengthInKm;
            existing.WalkImageUrl = walk.WalkImageUrl;
            existing.DifficultyId = walk.DifficultyId;
            existing.RegionId = walk.RegionId;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var walk = await _context.Walks.FirstOrDefaultAsync(x => x.Id == id);
            if (walk == null) return null;

            _context.Walks.Remove(walk);
            await _context.SaveChangesAsync();
            return walk;
        }
    }
}
