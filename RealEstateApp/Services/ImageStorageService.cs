using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace RealEstateApp.Services;

public record UploadedImage(string Url, string PublicId);

public interface IImageStorageService
{
    Task<UploadedImage> UploadAsync(IBrowserFile file, int propertyId);
    Task<UploadedImage> UploadAsync(IBrowserFile file, string subFolder);
    Task DeleteAsync(string? publicId);
    bool IsAllowed(IBrowserFile file, out string error);
}

public class CloudinaryImageStorageService : IImageStorageService
{
    public const long MaxBytes = 5 * 1024 * 1024;   // 5 MB
    private static readonly string[] Allowed = { ".jpg", ".jpeg", ".png", ".webp" };

    private readonly Cloudinary _cloudinary;
    private readonly string _rootFolder;

    public CloudinaryImageStorageService(IOptions<CloudinarySettings> options)
    {
        var s = options.Value;

        if (string.IsNullOrWhiteSpace(s.CloudName) ||
            string.IsNullOrWhiteSpace(s.ApiKey) ||
            string.IsNullOrWhiteSpace(s.ApiSecret))
        {
            throw new InvalidOperationException(
                "Cloudinary тохиргоо дутуу байна (CloudName / ApiKey / ApiSecret).");
        }

        _cloudinary = new Cloudinary(new Account(s.CloudName, s.ApiKey, s.ApiSecret));
        _cloudinary.Api.Secure = true;
        _rootFolder = string.IsNullOrWhiteSpace(s.Folder) ? "realestate" : s.Folder;
    }

    public bool IsAllowed(IBrowserFile file, out string error)
    {
        var ext = Path.GetExtension(file.Name).ToLowerInvariant();

        if (!Allowed.Contains(ext))
        {
            error = $"{file.Name}: зөвхөн JPG, PNG, WEBP зөвшөөрнө.";
            return false;
        }
        if (file.Size > MaxBytes)
        {
            error = $"{file.Name}: хэмжээ 5MB-аас их байна.";
            return false;
        }

        error = "";
        return true;
    }

    // Хөрөнгийн зураг — ерөнхий хувилбар руу дуудна
    public Task<UploadedImage> UploadAsync(IBrowserFile file, int propertyId)
        => UploadAsync(file, $"properties/{propertyId}");

    // Ерөнхий: дурын дэд фолдер руу (properties/12, agents/5 гэх мэт)
    public async Task<UploadedImage> UploadAsync(IBrowserFile file, string subFolder)
    {
        await using var stream = file.OpenReadStream(MaxBytes);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.Name, stream),
            Folder = $"{_rootFolder}/{subFolder}",
            UseFilename = false,
            UniqueFilename = true,
            Overwrite = false,
            // Хэт том зургийг хязгаарлаж, чанарыг автоматаар оновчилно
            Transformation = new Transformation()
                .Width(1600).Crop("limit").Quality("auto")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
            throw new InvalidOperationException($"Cloudinary: {result.Error.Message}");

        if (result.SecureUrl is null)
            throw new InvalidOperationException("Cloudinary: зургийн URL ирсэнгүй.");

        return new UploadedImage(result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task DeleteAsync(string? publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId)) return;
        await _cloudinary.DestroyAsync(new DeletionParams(publicId));
    }
}