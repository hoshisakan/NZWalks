using AutoMapper;
using NZWalks.Application.DTO;
using NZWalks.Application.Services;
using NZWalks.Domain.Repositories;

namespace NZWalks.Infrastructure.Services
{
    public class DifficultyService : IDifficultyService
    {
        private readonly IDifficultyRepository _difficultyRepository;
        private readonly IMapper _mapper;

        public DifficultyService(IDifficultyRepository difficultyRepository, IMapper mapper)
        {
            _difficultyRepository = difficultyRepository;
            _mapper = mapper;
        }

        public async Task<List<DifficultyDto>> GetAllAsync()
        {
            var list = await _difficultyRepository.GetAllAsync();
            return _mapper.Map<List<DifficultyDto>>(list);
        }

        public async Task<DifficultyDto> GetByIdAsync(Guid id)
        {
            var difficulty = await _difficultyRepository.GetByIdAsync(id);
            if (difficulty == null)
                throw new Exception("Difficulty not found!");
            return _mapper.Map<DifficultyDto>(difficulty);
        }

        public async Task<DifficultyDto> CreateAsync(AddDifficultyRequestDto dto)
        {
            var difficulty = _mapper.Map<Domain.Entities.Difficulty>(dto);
            difficulty = await _difficultyRepository.CreateAsync(difficulty);
            return _mapper.Map<DifficultyDto>(difficulty);
        }

        public async Task<DifficultyDto> UpdateAsync(Guid id, UpdateDifficultyRequestDto dto)
        {
            var difficulty = _mapper.Map<Domain.Entities.Difficulty>(dto);
            var updated = await _difficultyRepository.UpdateAsync(id, difficulty);
            if (updated == null)
                throw new Exception("Difficulty not found!");
            return _mapper.Map<DifficultyDto>(updated);
        }

        public async Task<DifficultyDto> DeleteAsync(Guid id)
        {
            var difficulty = await _difficultyRepository.DeleteAsync(id);
            if (difficulty == null)
                throw new Exception("Difficulty not found!");
            return _mapper.Map<DifficultyDto>(difficulty);
        }
    }
}
