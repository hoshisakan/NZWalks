using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NZWalks.API.Repositories;
using NZWalks.API.Models.DTO;
using NZWalks.API.Models.Domain;
using AutoMapper;


namespace NZWalks.API.Services
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

        public async Task<ImageDto> UploadAsync(ImageUploadRequestDto imageUploadRequestDto)
        {
            var image = new Image
            {
                File = imageUploadRequestDto.File,
                FileName = imageUploadRequestDto.FileName,
                FileDescription = imageUploadRequestDto.FileDescription,
                FileExtension = Path.GetExtension(imageUploadRequestDto.File.FileName),
                FileSizeInBytes = imageUploadRequestDto.File.Length,
                // FilePath = imageUploadRequestDto.File.FileName
            };
            var result = await _imageRepository.UploadAsync(image);
            return _mapper.Map<ImageDto>(result);
        }
    }
}