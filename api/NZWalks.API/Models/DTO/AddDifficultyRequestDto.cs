using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace NZWalks.API.Models.DTO
{
    public class AddDifficultyRequestDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Name must be at most 100 characters long")]
        public string Name { get; set; }
    }
}