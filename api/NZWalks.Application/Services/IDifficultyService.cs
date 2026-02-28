using NZWalks.Application.DTO;

namespace NZWalks.Application.Services
{
    public interface IDifficultyService
    {
        Task<List<DifficultyDto>> GetAllAsync();
        Task<DifficultyDto> GetByIdAsync(Guid id);
        Task<DifficultyDto> CreateAsync(AddDifficultyRequestDto addDifficultyRequestDto);
        Task<DifficultyDto> UpdateAsync(Guid id, UpdateDifficultyRequestDto updateDifficultyRequestDto);
        Task<DifficultyDto> DeleteAsync(Guid id);
    }
}
