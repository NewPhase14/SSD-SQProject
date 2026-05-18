using Application.Models.Dtos.Cloudinary;

namespace Application.Interfaces;

public interface ICloudinaryImageService
{
    Task<CloudinaryUploadResponseDto> UploadImageAsync(Stream fileStream, string fileName);

    Task DeleteImagesAsync(List<string> publicIds);
}