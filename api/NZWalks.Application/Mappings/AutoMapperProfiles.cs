using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NZWalks.Domain.Entities;
using NZWalks.Application.DTO;

namespace NZWalks.Application.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Region, RegionDto>().ReverseMap();
            CreateMap<Region, AddRegionRequestDto>().ReverseMap();
            CreateMap<Region, UpdateRegionRequestDto>().ReverseMap();
            CreateMap<Walk, WalkDto>().ReverseMap();
            CreateMap<Walk, AddWalkRequestDto>().ReverseMap();
            CreateMap<Walk, UpdateWalkRequestDto>().ReverseMap();
            CreateMap<Difficulty, DifficultyDto>().ReverseMap();
            CreateMap<Difficulty, AddDifficultyRequestDto>().ReverseMap();
            CreateMap<Difficulty, UpdateDifficultyRequestDto>().ReverseMap();
            CreateMap<Image, ImageDto>().ReverseMap();
            CreateMap<IdentityUser, RegisterResponseDto>()
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.UserName))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email));
        }
    }
}
