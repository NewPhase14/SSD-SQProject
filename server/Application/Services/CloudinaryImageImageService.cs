using Application.Interfaces;
using Application.Models.Dtos.Cloudinary;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Application.Services;

public class CloudinaryImageImageService(Cloudinary cloudinary) : ICloudinaryImageService
{
    // Uploads an image asynchronously to Cloudinary and returns the image URL (URL is the link to the uploaded image)
    public async Task<CloudinaryUploadResponseDto> UploadImageAsync(Stream fileStream, string fileName)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill"),
            // PublicId is the unique identifier for the image in Cloudinary (same as "filename")
            PublicId = fileName
        };

        if (uploadParams == null) throw new ArgumentNullException(nameof(uploadParams));

        var uploadResult = await cloudinary.UploadAsync(uploadParams);
        if (uploadResult == null) throw new InvalidOperationException("Upload failed");

        // Return the dto with HTTPS URL of the uploaded image and the public ID, so the values can be used in database
        return new CloudinaryUploadResponseDto
        {
            SecureUrl = uploadResult.SecureUrl.ToString(),
            PublicId = uploadResult.PublicId
        };
    }

    public async Task DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image
        };

        var deletionResult = await cloudinary.DestroyAsync(deleteParams);
        if (deletionResult == null) throw new InvalidOperationException("Image deletion failed");
    }
}