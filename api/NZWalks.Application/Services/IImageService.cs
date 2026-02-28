using NZWalks.Application.DTO;

namespace NZWalks.Application.Services
{
    public interface IImageService
    {
        Task<ImageDto> UploadAsync(ImageUploadRequestDto imageUploadRequestDto);
    }
}
