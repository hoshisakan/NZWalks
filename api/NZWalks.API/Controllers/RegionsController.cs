using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using NZWalks.API.CustomActionFilters;
using Microsoft.AspNetCore.Identity;
using NZWalks.API.Services;


namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _regionService;

        public RegionsController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpGet]
        [Authorize(Roles = "Reader,Admin")]
        public async Task<IActionResult> GetAll()
        {
            var regions = await _regionService.GetAllAsync();
            return Ok(regions);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles = "Reader,Admin")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var region = await _regionService.GetByIdAsync(id);
            return Ok(region);
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            var region = await _regionService.CreateAsync(addRegionRequestDto);
            return CreatedAtAction(nameof(GetById), new { id = region.Id }, region);
        }

        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            var region = await _regionService.UpdateAsync(id, updateRegionRequestDto);
            return Ok(region);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var region = await _regionService.DeleteAsync(id);
            return Ok(region);
        }
    }
}