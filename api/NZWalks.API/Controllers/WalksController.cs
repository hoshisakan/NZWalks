using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Services;
using NZWalks.API.Models.DTO;


namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WalksController : ControllerBase
    {
        private readonly IWalkService _walkService;

        public WalksController(IWalkService walkService)
        {
            _walkService = walkService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
        [FromQuery] string? sortBy, [FromQuery] bool isAscending,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var walks = await _walkService.GetAllAsync(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);

            return Ok(walks);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walk = await _walkService.GetByIdAsync(id);
            return Ok(walk);
        }
        
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            var walk = await _walkService.CreateAsync(addWalkRequestDto);
            return CreatedAtAction(nameof(GetById), new { id = walk.Id }, walk);
        }

        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
        {
            var walk = await _walkService.UpdateAsync(id, updateWalkRequestDto);
            return Ok(walk);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walk = await _walkService.DeleteAsync(id);
            return Ok(walk);
        }
    }
}