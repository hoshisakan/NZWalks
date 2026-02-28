using AutoMapper;
using NZWalks.Application.DTO;
using NZWalks.Application.Services;
using NZWalks.Domain.Repositories;

namespace NZWalks.Infrastructure.Services
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
            var walks = await _walkRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
            return _mapper.Map<List<WalkDto>>(walks);
        }

        public async Task<WalkDto> GetByIdAsync(Guid id)
        {
            var walk = await _walkRepository.GetByIdAsync(id);
            if (walk == null)
                throw new Exception("Walk not found!");
            return _mapper.Map<WalkDto>(walk);
        }

        public async Task<WalkDto> CreateAsync(AddWalkRequestDto dto)
        {
            var walk = _mapper.Map<Domain.Entities.Walk>(dto);
            walk = await _walkRepository.CreateAsync(walk);
            return _mapper.Map<WalkDto>(walk);
        }

        public async Task<WalkDto> UpdateAsync(Guid id, UpdateWalkRequestDto dto)
        {
            var walk = _mapper.Map<Domain.Entities.Walk>(dto);
            var updated = await _walkRepository.UpdateAsync(id, walk);
            if (updated == null)
                throw new Exception("Walk not found!");
            return _mapper.Map<WalkDto>(updated);
        }

        public async Task<WalkDto> DeleteAsync(Guid id)
        {
            var walk = await _walkRepository.DeleteAsync(id);
            if (walk == null)
                throw new Exception("Walk not found!");
            return _mapper.Map<WalkDto>(walk);
        }
    }
}
