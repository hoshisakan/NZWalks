using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Services
{
    public interface IImageService
    {
        Task<ImageDto> UploadAsync(ImageUploadRequestDto imageUploadRequestDto);
    }
}