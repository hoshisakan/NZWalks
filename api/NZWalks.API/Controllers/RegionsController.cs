using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using NZWalks.API.CustomActionFilters;


namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;

        public RegionsController(IRegionRepository regionRepository, IMapper mapper)
        {
            _regionRepository = regionRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var regions = await _regionRepository.GetAllAsync();

            var regionsDto = _mapper.Map<List<RegionDto>>(regions);

            return Ok(regionsDto);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var region = await _regionRepository.GetByIdAsync(id);

            if (region == null)
            {
                return NotFound();
            }

            var regionDto = _mapper.Map<RegionDto>(region);

            return Ok(regionDto);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            var region = _mapper.Map<Region>(addRegionRequestDto);

            region = await _regionRepository.CreateAsync(region);

            var regionDto = _mapper.Map<RegionDto>(region);

            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            var region = _mapper.Map<Region>(updateRegionRequestDto);

            region = await _regionRepository.UpdateAsync(id, region);

            if (region == null)
            {
                return NotFound("Region not found");
            }

            var regionDto = _mapper.Map<RegionDto>(region);

            return Ok(regionDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var region = await _regionRepository.DeleteAsync(id);

            if (region == null)
            {
                return NotFound("Region not found");
            }

            var regionDto = _mapper.Map<RegionDto>(region);

            return Ok(regionDto);
        }
    }
}