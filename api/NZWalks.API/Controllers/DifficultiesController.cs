using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Data;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using NZWalks.API.CustomActionFilters;
using Microsoft.AspNetCore.Identity;


namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DifficultiesController : ControllerBase
    {
        private readonly IDifficultyRepository _difficultyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DifficultiesController> _logger;

        public DifficultiesController(IDifficultyRepository difficultyRepository, IMapper mapper, ILogger<DifficultiesController> logger)
        {
            _difficultyRepository = difficultyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {
            var difficulties = await _difficultyRepository.GetAllAsync();

            var difficultiesDto = _mapper.Map<List<DifficultyDto>>(difficulties);

            return Ok(difficultiesDto);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var difficulty = await _difficultyRepository.GetByIdAsync(id);

            if (difficulty == null)
            {
                return NotFound();
            }

            var difficultyDto = _mapper.Map<DifficultyDto>(difficulty);

            return Ok(difficultyDto);
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddDifficultyRequestDto addDifficultyRequestDto)
        {
            var difficulty = _mapper.Map<Difficulty>(addDifficultyRequestDto);
            
            difficulty = await _difficultyRepository.CreateAsync(difficulty);

            var difficultyDto = _mapper.Map<DifficultyDto>(difficulty);

            return CreatedAtAction(nameof(GetById), new { id = difficultyDto.Id }, difficultyDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateDifficultyRequestDto updateDifficultyRequestDto)
        {
            var difficulty = _mapper.Map<Difficulty>(updateDifficultyRequestDto);
            
            difficulty = await _difficultyRepository.UpdateAsync(id, difficulty);

            if (difficulty == null)
            {
                return NotFound();
            }

            var difficultyDto = _mapper.Map<DifficultyDto>(difficulty);

            return Ok(difficultyDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var difficulty = await _difficultyRepository.DeleteAsync(id);

            if (difficulty == null)
            {
                return NotFound();
            }

            var difficultyDto = _mapper.Map<DifficultyDto>(difficulty);

            return Ok(difficultyDto);
        }
    }
}