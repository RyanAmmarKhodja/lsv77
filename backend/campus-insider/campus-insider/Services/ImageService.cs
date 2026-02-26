using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace campus_insider.Services
{
    public class ImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private const int MaxWidth = 1920;
        private const int MaxHeight = 1920;

        public ImageService(IConfiguration configuration)
        {
            var acc = new Account(
                configuration["CloudinarySettings:CloudName"],
                configuration["CloudinarySettings:ApiKey"],
                configuration["CloudinarySettings:ApiSecret"]
            );
            _cloudinary = new Cloudinary(acc);
            _cloudinary.Api.Secure = true;
        }

        public async Task<ServiceResult<ImageUploadResult>> UploadEquipmentImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return ServiceResult<ImageUploadResult>.Fail("No file provided.");

            if (file.Length > MaxFileSize)
                return ServiceResult<ImageUploadResult>.Fail($"File size must be less than {MaxFileSize / 1024 / 1024}MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return ServiceResult<ImageUploadResult>.Fail($"Only {string.Join(", ", _allowedExtensions)} files are allowed.");

            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "equipment",
                    // Offload processing to Cloudinary to save your server CPU
                    Transformation = new Transformation()
                        .Width(MaxWidth).Height(MaxHeight).Crop("limit")
                        .Quality("auto").FetchFormat("auto")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    return ServiceResult<ImageUploadResult>.Fail(uploadResult.Error.Message);

                return ServiceResult<ImageUploadResult>.Ok(new ImageUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    FileName = uploadResult.PublicId,
                    OriginalFileName = file.FileName,
                    Url = uploadResult.SecureUrl.AbsoluteUri,
                    FileSize = uploadResult.Length
                });
            }
            catch (Exception ex)
            {
                return ServiceResult<ImageUploadResult>.Fail($"Cloudinary upload failed: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteEquipmentImage(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return ServiceResult.Fail("Invalid Public ID.");

            try
            {
                var deletionParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deletionParams);

                return result.Result == "ok"
                    ? ServiceResult.Ok()
                    : ServiceResult.Fail($"Cloudinary delete failed: {result.Result}");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Failed to delete image: {ex.Message}");
            }
        }
    }

    public class ImageUploadResult
    {
        public string PublicId { get; set; } = string.Empty; // Store this for deletion
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }
}