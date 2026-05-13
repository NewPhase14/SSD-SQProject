using Application.Interfaces;
using SixLabors.ImageSharp;

namespace Application.Services;

public class FileValidationService : IFileValidationService
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    private static readonly string[] AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png"
    ];
    
    private static readonly string[] AllowedMimeTypes =
    [
        "image/jpg",
        "image/jpeg",
        "image/png"
    ];
    
    public async Task ValidateImageAsync(Stream fileStream, string fileName, string contentType)
    {
        // 1. File size check 
        if (fileStream.Length > MaxFileSize)
            throw new InvalidOperationException("File size exceeds 5MB limit.");

        // 2. Extension check
        var extension = Path.GetExtension(fileName).ToLower();

        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Invalid file extension.");
        
        // 3. MIME type check
        if (string.IsNullOrWhiteSpace(contentType) ||
            !AllowedMimeTypes.Contains(contentType))
        {
            throw new InvalidOperationException("Invalid MIME type.");
        }
        
        // Image validation  
        try
        {
            fileStream.Position = 0;

            using var image = await Image.LoadAsync(fileStream);

            if (image.Width <= 0 || image.Height <= 0)
                throw new InvalidOperationException("Invalid image file.");
        }
        catch
        {
            throw new InvalidOperationException("File is not a valid image.");
        }

        // Reset stream so it can be used later 
        fileStream.Position = 0;
    }
}