using System.Text;
using Application.Interfaces;
using Application.Services;

namespace UnitTests.FileValidationTests;

public class FileValidationServiceTests
{
    private readonly IFileValidationService _fileValidationService = new FileValidationService();
    
    [Fact]
    public async Task ValidateImageAsync_InvalidExtension_ThrowsException()
    {
        using var stream = new MemoryStream(new byte[100]);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _fileValidationService.ValidateImageAsync(stream, "virus.exe", "image/png"));
    }

    [Fact]
    public async Task ValidateImageAsync_InvalidMimeType_ThrowsException()
    {
        using var stream = new MemoryStream(new byte[100]);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _fileValidationService.ValidateImageAsync(stream, "image.jpg", "application/pdf"));
    }

    [Fact]
    public async Task ValidateImageAsync_FileTooLarge_ThrowsException()
    {
        var largeBytes = new byte[6 * 1024 * 1024];
        using var stream = new MemoryStream(largeBytes);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _fileValidationService.ValidateImageAsync(stream, "image.jpg", "image/jpeg"));
    }

    [Fact]
    public async Task ValidateImageAsync_InvalidImageContent_ThrowsException()
    {
        var fakeImage = Encoding.UTF8.GetBytes("not an image");

        using var stream = new MemoryStream(fakeImage);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _fileValidationService.ValidateImageAsync(stream, "image.jpg", "image/jpeg"));
    }
    
}