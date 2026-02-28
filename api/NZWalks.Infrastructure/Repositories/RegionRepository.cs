using Microsoft.EntityFrameworkCore;
using NZWalks.Domain.Entities;
using NZWalks.Domain.Repositories;
using NZWalks.Infrastructure.Data;

namespace NZWalks.Infrastructure.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly NZWalksDbContext _context;

        public RegionRepository(NZWalksDbContext context)
        {
            _context = context;
        }

        public async Task<List<Region>> GetAllAsync()
        {
            return await _context.Regions.ToListAsync();
        }

        public async Task<Region?> GetByIdAsync(Guid id)
        {
            return await _context.Regions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Region> CreateAsync(Region region)
        {
            await _context.Regions.AddAsync(region);
            await _context.SaveChangesAsync();
            return region;
        }

        public async Task<Region?> UpdateAsync(Guid id, Region region)
        {
            var existing = await _context.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return null;

            existing.Code = region.Code;
            existing.Name = region.Name;
            existing.RegionImageUrl = region.RegionImageUrl;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<Region?> DeleteAsync(Guid id)
        {
            var region = await _context.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if (region == null) return null;

            _context.Regions.Remove(region);
            await _context.SaveChangesAsync();
            return region;
        }
    }
}
