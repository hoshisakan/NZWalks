using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NZWalks.Domain.Entities;
using NZWalks.Domain.Repositories;
using NZWalks.Infrastructure.Data;

namespace NZWalks.Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly NZWalksDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageRepository(NZWalksDbContext context, IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Image> UploadAsync(Image image, Stream fileStream)
        {
            var checkDirectory = Path.Combine(_webHostEnvironment.ContentRootPath, "Images");
            if (!Directory.Exists(checkDirectory))
                Directory.CreateDirectory(checkDirectory);

            var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images",
                $"{image.FileName}{image.FileExtension}");

            await using (var stream = new FileStream(localFilePath, FileMode.Create))
                await fileStream.CopyToAsync(stream);

            var request = _httpContextAccessor.HttpContext?.Request;
            var urlFilePath = $"{request?.Scheme}://{request?.Host}{request?.PathBase}/Images/{image.FileName}{image.FileExtension}";
            image.FilePath = urlFilePath;

            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
            return image;
        }
    }
}
