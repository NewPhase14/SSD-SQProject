namespace Application.Models.Dtos.Cloudinary;

public class CloudinaryUploadResponseDto
{
    public string PublicId { get; set; } = null!;
    public string SecureUrl { get; set; } = null!;
}