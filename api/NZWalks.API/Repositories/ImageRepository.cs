using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NZWalks.API.Models.Domain;
using NZWalks.API.Data;


namespace NZWalks.API.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly NZWalksDbContext _nZWalksDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageRepository(NZWalksDbContext nZWalksDbContext, IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            _nZWalksDbContext = nZWalksDbContext;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Image> UploadAsync(Image image)
        {
            var localFilePath = Path.Combine(
                _webHostEnvironment.ContentRootPath,
                "Images", $"{image.FileName}{image.FileExtension}"
            );

            using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);

            var urlFilePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

            image.FilePath = urlFilePath;

            await _nZWalksDbContext.Images.AddAsync(image);

            await _nZWalksDbContext.SaveChangesAsync();

            return image;
        }
    }
}