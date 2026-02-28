using NZWalks.Application.DTO;

namespace NZWalks.Application.Services
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
