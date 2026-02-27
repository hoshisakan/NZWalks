using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NZWalks.API.Models.DTO;


namespace NZWalks.API.Services
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