using System.Text;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Cloudinary;
using Application.Models.Dtos.Listings;
using Application.Services;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UnitTests.ListingTests;

public class ListingServiceTests
{
    
    private readonly Mock<IListingRepo> _mockListingRepo;
    private readonly Mock<ICloudinaryImageService> _mockCloudinaryImageService;
    private readonly Mock<IFileValidationService> _mockFileValidationService;
    private readonly IListingService _listingService;
    
    public ListingServiceTests()
    {
        _mockListingRepo = new Mock<IListingRepo>();
        _mockCloudinaryImageService = new Mock<ICloudinaryImageService>();
        _mockFileValidationService = new Mock<IFileValidationService>();
        
        _listingService = new ListingService(
            _mockListingRepo.Object,
            _mockCloudinaryImageService.Object,
            _mockFileValidationService.Object);
    }

    [Fact]
    public async Task CreateListingAsync_ValidInput_ReturnsCreatedListing()
    {
        // Arrange — mock a valid image file
        var ms = new MemoryStream(Encoding.UTF8.GetBytes("fake image content"));
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.FileName).Returns("test.jpg");
        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.ContentType).Returns("image/jpeg");

        _mockCloudinaryImageService
            .Setup(x => x.UploadImageAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(new CloudinaryUploadResponseDto
            {
                SecureUrl = "https://cloudinary.com/image.jpg",
                PublicId = "public-id"
            });

        _mockListingRepo
            .Setup(x => x.CreateListingAsync(It.IsAny<Listing>()))
            .ReturnsAsync((Listing listing) => listing);

        // Act
        var result = await _listingService.CreateListingAsync(new ListingCreateRequestDto
        {
            CategoryId = "cat-1",
            Condition = "Used",
            Title = "Laptop",
            Description = "Gaming laptop",
            Price = 1000,
            Status = "Active",
            Images = [fileMock.Object]
        }, "user-1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Laptop", result.Title);
        Assert.Single(result.ImageUrls);

        // Verify validation and upload were both called exactly once
        _mockFileValidationService.Verify(
            x => x.ValidateImageAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);

        _mockCloudinaryImageService.Verify(
            x => x.UploadImageAsync(It.IsAny<Stream>(), It.IsAny<string>()),
            Times.Once);
    }
    
    [Fact]
    public async Task CreateListingAsync_InvalidFile_ThrowsException()
    {
        // Arrange — file validation rejects the file before it reaches Cloudinary
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
        fileMock.Setup(f => f.FileName).Returns("virus.exe");
        fileMock.Setup(f => f.Length).Returns(100);
        fileMock.Setup(f => f.ContentType).Returns("application/octet-stream");

        _mockFileValidationService
            .Setup(x => x.ValidateImageAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Invalid file"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _listingService.CreateListingAsync(new ListingCreateRequestDto
            {
                Images = [fileMock.Object]
            }, "user-1"));
    }
    
    [Fact]
    public async Task DeleteListingAsync_DeletesCloudinaryImages()
    {
        // Arrange — listing has two images that must be cleaned up from Cloudinary on delete
        var listing = new Listing
        {
            Id = "listing-1",
            UserId = "owner-1",
            Images =
            [
                new Image { PublicId = "img-1" },
                new Image { PublicId = "img-2" }
            ]
        };

        _mockListingRepo
            .Setup(x => x.GetListingByIdAsync("listing-1"))
            .ReturnsAsync(listing);

        _mockListingRepo
            .Setup(x => x.DeleteListingAsync("listing-1"))
            .ReturnsAsync(listing);

        // Act
        var result = await _listingService.DeleteListingAsync("listing-1", "owner-1");

        // Assert — both Cloudinary public IDs must be passed to the delete call
        Assert.NotNull(result);
        _mockCloudinaryImageService.Verify(
            x => x.DeleteImagesAsync(It.Is<List<string>>(ids =>
                ids.Contains("img-1") && ids.Contains("img-2"))),
            Times.Once);
    }
    
}