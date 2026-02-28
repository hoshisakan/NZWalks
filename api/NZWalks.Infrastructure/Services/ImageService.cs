using AutoMapper;
using NZWalks.Application.DTO;
using NZWalks.Application.Services;
using NZWalks.Domain.Entities;
using NZWalks.Domain.Repositories;

namespace NZWalks.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public ImageService(IImageRepository imageRepository, IMapper mapper)
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        public async Task<ImageDto> UploadAsync(ImageUploadRequestDto dto)
        {
            var image = new Image
            {
                FileName = dto.FileName,
                FileDescription = dto.FileDescription ?? "",
                FileExtension = Path.GetExtension(dto.File.FileName),
                FileSizeInBytes = dto.File.Length
            };
            await using var stream = dto.File.OpenReadStream();
            var result = await _imageRepository.UploadAsync(image, stream);
            return _mapper.Map<ImageDto>(result);
        }
    }
}
