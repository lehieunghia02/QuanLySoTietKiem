using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using QuanLySoTietKiem.Configurations;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Services.Implementations
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _cloudinarySettings;
        public CloudinaryService(
        IOptions<CloudinarySettings> cloudinarySettings)
        {
            _cloudinarySettings = cloudinarySettings.Value;
            var account = new Account(
            _cloudinarySettings.CloudName,
            _cloudinarySettings.ApiKey,
            _cloudinarySettings.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("No file uploaded");
                }
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new ArgumentException("Invalid file extension");
                }
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "picture_savings",
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }
                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrEmpty(publicId))
                {
                    throw new ArgumentException("Public ID is required");
                }
                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image
                };
                var result = await _cloudinary.DestroyAsync(deleteParams);
                if (result.Error != null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
