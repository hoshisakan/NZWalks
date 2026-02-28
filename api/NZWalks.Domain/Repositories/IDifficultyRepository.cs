using NZWalks.Domain.Entities;

namespace NZWalks.Domain.Repositories
{
    public interface IDifficultyRepository
    {
        Task<List<Difficulty>> GetAllAsync();
        Task<Difficulty?> GetByIdAsync(Guid id);
        Task<Difficulty> CreateAsync(Difficulty difficulty);
        Task<Difficulty?> UpdateAsync(Guid id, Difficulty difficulty);
        Task<Difficulty?> DeleteAsync(Guid id);
    }
}
