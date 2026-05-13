namespace Application.Interfaces;

public interface IFileValidationService
{
    public Task ValidateImageAsync(Stream fileStream, string fileName, string contentType);
}