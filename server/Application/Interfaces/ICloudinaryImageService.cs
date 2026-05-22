using Application.Models.Dtos.Cloudinary;

namespace Application.Interfaces;

public interface ICloudinaryImageService
{
    public Task<CloudinaryUploadResponseDto> UploadImageAsync(Stream fileStream, string fileName);

    public Task DeleteImagesAsync(List<string> publicIds);
}