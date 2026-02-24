using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NZWalks.API.Models.Domain;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;


namespace NZWalks.API.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly NZWalksDbContext _nZWalksDbContext;

        public RegionRepository(NZWalksDbContext nZWalksDbContext)
        {
            _nZWalksDbContext = nZWalksDbContext;
        }

        public async Task<List<Region>> GetAllAsync()
        {
            return await _nZWalksDbContext.Regions.ToListAsync();
        }

        public async Task<Region> GetByIdAsync(Guid id)
        {
            return await _nZWalksDbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Region> CreateAsync(Region region)
        {
            await _nZWalksDbContext.Regions.AddAsync(region);
            await _nZWalksDbContext.SaveChangesAsync();
            return region;
        }
        
        public async Task<Region> UpdateAsync(Guid id, Region region)
        {
            var existingRegion = await _nZWalksDbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (existingRegion == null)
            {
                return null;
            }

            existingRegion.Code = region.Code;
            existingRegion.Name = region.Name;
            existingRegion.RegionImageUrl = region.RegionImageUrl;

            await _nZWalksDbContext.SaveChangesAsync();

            return existingRegion;
        }

        public async Task<Region> DeleteAsync(Guid id)
        {
            var region = await _nZWalksDbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (region == null)
            {
                return null;
            }

            _nZWalksDbContext.Regions.Remove(region);
            await _nZWalksDbContext.SaveChangesAsync();
            return region;
        }
    }
}