using NZWalks.Domain.Entities;

namespace NZWalks.Domain.Repositories
{
    public interface IImageRepository
    {
        Task<Image> UploadAsync(Image image, Stream fileStream);
    }
}
