using AutoMapper;
using NZWalks.Application.DTO;
using NZWalks.Application.Services;
using NZWalks.Domain.Repositories;

namespace NZWalks.Infrastructure.Services
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;

        public RegionService(IRegionRepository regionRepository, IMapper mapper)
        {
            _regionRepository = regionRepository;
            _mapper = mapper;
        }

        public async Task<List<RegionDto>> GetAllAsync()
        {
            var regions = await _regionRepository.GetAllAsync();
            return _mapper.Map<List<RegionDto>>(regions);
        }

        public async Task<RegionDto> GetByIdAsync(Guid id)
        {
            var region = await _regionRepository.GetByIdAsync(id);
            if (region == null)
                throw new Exception("Region not found!");
            return _mapper.Map<RegionDto>(region);
        }

        public async Task<RegionDto> CreateAsync(AddRegionRequestDto dto)
        {
            var region = _mapper.Map<Domain.Entities.Region>(dto);
            region = await _regionRepository.CreateAsync(region);
            return _mapper.Map<RegionDto>(region);
        }

        public async Task<RegionDto> UpdateAsync(Guid id, UpdateRegionRequestDto dto)
        {
            var region = _mapper.Map<Domain.Entities.Region>(dto);
            var updated = await _regionRepository.UpdateAsync(id, region);
            if (updated == null)
                throw new Exception("Region not found!");
            return _mapper.Map<RegionDto>(updated);
        }

        public async Task<RegionDto> DeleteAsync(Guid id)
        {
            var region = await _regionRepository.DeleteAsync(id);
            if (region == null)
                throw new Exception("Region not found!");
            return _mapper.Map<RegionDto>(region);
        }
    }
}
