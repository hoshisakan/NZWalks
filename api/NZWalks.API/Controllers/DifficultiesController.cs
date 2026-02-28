using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using NZWalks.API.CustomActionFilters;
using NZWalks.Application.Services;
using Asp.Versioning;


namespace NZWalks.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class DifficultiesController : ControllerBase
    {
        private readonly IDifficultyService _difficultyService;

        public DifficultiesController(IDifficultyService difficultyService)
        {
            _difficultyService = difficultyService;
        }

        [HttpGet]
        [Authorize(Roles = "Reader,Admin")]
        public async Task<IActionResult> GetAll()
        {
            var difficulties = await _difficultyService.GetAllAsync();
            return Ok(difficulties);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles = "Reader,Admin")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var difficulty = await _difficultyService.GetByIdAsync(id);
            return Ok(difficulty);
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> Create([FromBody] AddDifficultyRequestDto addDifficultyRequestDto)
        {
            var difficulty = await _difficultyService.CreateAsync(addDifficultyRequestDto);
            return CreatedAtAction(nameof(GetById), new { id = difficulty.Id }, difficulty);
        }

        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateDifficultyRequestDto updateDifficultyRequestDto)
        {
            var difficulty = await _difficultyService.UpdateAsync(id, updateDifficultyRequestDto);
            return Ok(difficulty);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var difficulty = await _difficultyService.DeleteAsync(id);
            return Ok(difficulty);
        }
    }
}