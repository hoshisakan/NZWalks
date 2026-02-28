using NZWalks.Application.DTO;

namespace NZWalks.Application.Services
{
    public interface IWalkService
    {
        Task<List<WalkDto>> GetAllAsync(string? filterOn, string? filterQuery,
            string? sortBy, bool isAscending, int pageNumber = 1, int pageSize = 1000);
        Task<WalkDto> GetByIdAsync(Guid id);
        Task<WalkDto> CreateAsync(AddWalkRequestDto addWalkRequestDto);
        Task<WalkDto> UpdateAsync(Guid id, UpdateWalkRequestDto updateWalkRequestDto);
        Task<WalkDto> DeleteAsync(Guid id);
    }
}
