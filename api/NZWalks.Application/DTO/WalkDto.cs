namespace NZWalks.Application.DTO
{
    public class WalkDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double LengthInKm { get; set; }
        public string? WalkImageUrl { get; set; }
        public Guid DifficultyId { get; set; }
        public Guid RegionId { get; set; }
        public DifficultyDto Difficulty { get; set; } = null!;
        public RegionDto Region { get; set; } = null!;
    }
}
