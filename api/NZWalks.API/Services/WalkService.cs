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
    public class WalkService : IWalkService
    {
        private readonly IWalkRepository _walkRepository;
        private readonly IMapper _mapper;

        public WalkService(IWalkRepository walkRepository, IMapper mapper)
        {
            _walkRepository = walkRepository;
            _mapper = mapper;
        }

        public async Task<List<WalkDto>> GetAllAsync(string? filterOn, string? filterQuery,
        string? sortBy, bool isAscending, int pageNumber = 1, int pageSize = 1000)
        {
            var walks = await _walkRepository.GetAllAsync(filterOn, filterQuery, sortBy, 
                isAscending, pageNumber, pageSize);

            return _mapper.Map<List<WalkDto>>(walks);
        }

        public async Task<WalkDto> GetByIdAsync(Guid id)
        {
            var walk = await _walkRepository.GetByIdAsync(id);
            
            if (walk == null)
            {
                throw new Exception("Walk not found!");
            }
            return _mapper.Map<WalkDto>(walk);
        }
        
        public async Task<WalkDto> CreateAsync(AddWalkRequestDto addWalkRequestDto)
        {
            var walk = _mapper.Map<Walk>(addWalkRequestDto);
            walk = await _walkRepository.CreateAsync(walk);
            return _mapper.Map<WalkDto>(walk);
        }
        
        
        public async Task<WalkDto> UpdateAsync(Guid id, UpdateWalkRequestDto updateWalkRequestDto)
        {
            var walk = _mapper.Map<Walk>(updateWalkRequestDto);
            walk = await _walkRepository.UpdateAsync(id, walk);
            if (walk == null)
            {
                throw new Exception("Walk not found!");
            }
            return _mapper.Map<WalkDto>(walk);
        }

        public async Task<WalkDto> DeleteAsync(Guid id)
        {
            var walk = await _walkRepository.DeleteAsync(id);
            return _mapper.Map<WalkDto>(walk);
            if (walk == null)
            {
                throw new Exception("Walk not found!");
            }
            return _mapper.Map<WalkDto>(walk);
        }
    }
}