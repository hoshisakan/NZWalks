using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using NZWalks.API.CustomActionFilters;


namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IWalkRepository _walkRepository;
        private readonly IMapper _mapper;

        public WalksController(IWalkRepository walkRepository, IMapper mapper)
        {
            _walkRepository = walkRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var walks = await _walkRepository.GetAllAsync();

            var walksDto = _mapper.Map<List<WalkDto>>(walks);

            return Ok(walksDto);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walk = await _walkRepository.GetByIdAsync(id);

            if (walk == null)
            {
                return NotFound();
            }

            var walkDto = _mapper.Map<WalkDto>(walk);

            return Ok(walkDto);
        }
        
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            var walk = _mapper.Map<Walk>(addWalkRequestDto);

            walk = await _walkRepository.CreateAsync(walk);

            var walkDto = _mapper.Map<WalkDto>(walk);

            return CreatedAtAction(nameof(GetById), new { id = walkDto.Id }, walkDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
        {
            var walk = _mapper.Map<Walk>(updateWalkRequestDto);

            walk = await _walkRepository.UpdateAsync(id, walk);

            if (walk == null)
            {
                return NotFound("Walk not found");
            }

            var walkDto = _mapper.Map<WalkDto>(walk);

            return Ok(walkDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walk = await _walkRepository.DeleteAsync(id);

            if (walk == null)
            {
                return NotFound("Walk not found");
            }

            var walkDto = _mapper.Map<WalkDto>(walk);

            return Ok(walkDto);
        }
    }
}