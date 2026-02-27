using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NZWalks.API.Models.DTO;


namespace NZWalks.API.Services
{
    public interface IRegionService
    {
        Task<List<RegionDto>> GetAllAsync();
        Task<RegionDto> GetByIdAsync(Guid id);
        Task<RegionDto> CreateAsync(AddRegionRequestDto addRegionRequestDto);
        Task<RegionDto> UpdateAsync(Guid id, UpdateRegionRequestDto updateRegionRequestDto);
        Task<RegionDto> DeleteAsync(Guid id);
    }
}