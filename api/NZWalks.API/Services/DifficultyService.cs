using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NZWalks.API.Repositories;
using NZWalks.API.Models.DTO;
using NZWalks.API.Models.Domain;
using AutoMapper;


namespace NZWalks.API.Services
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
            var difficulties = await _difficultyRepository.GetAllAsync();
            return _mapper.Map<List<DifficultyDto>>(difficulties);
        }

        public async Task<DifficultyDto> GetByIdAsync(Guid id)
        {
            var difficulty = await _difficultyRepository.GetByIdAsync(id);
            
            if (difficulty == null)
            {
                throw new Exception("Difficulty not found!");
            }
            return _mapper.Map<DifficultyDto>(difficulty);
        }

        public async Task<DifficultyDto> CreateAsync(AddDifficultyRequestDto addDifficultyRequestDto)
        {
            var difficulty = _mapper.Map<Difficulty>(addDifficultyRequestDto);
            difficulty = await _difficultyRepository.CreateAsync(difficulty);
            return _mapper.Map<DifficultyDto>(difficulty);
        }

        public async Task<DifficultyDto> UpdateAsync(Guid id, UpdateDifficultyRequestDto updateDifficultyRequestDto)
        {
            var difficulty = _mapper.Map<Difficulty>(updateDifficultyRequestDto);
            difficulty = await _difficultyRepository.UpdateAsync(id, difficulty);
            if (difficulty == null)
            {
                throw new Exception("Difficulty not found!");
            }
            return _mapper.Map<DifficultyDto>(difficulty);
        }

        public async Task<DifficultyDto> DeleteAsync(Guid id)
        {
            var difficulty = await _difficultyRepository.DeleteAsync(id);
            if (difficulty == null)
            {
                throw new Exception("Difficulty not found!");
            }
            return _mapper.Map<DifficultyDto>(difficulty);
        }
    }
}