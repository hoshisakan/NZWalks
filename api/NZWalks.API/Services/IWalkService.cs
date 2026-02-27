using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NZWalks.API.Models.DTO;


namespace NZWalks.API.Services
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