using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Listings;
using Microsoft.AspNetCore.Http;
using Reqnroll;
using Moq;
using Application.Models.Dtos.Cloudinary;
using Application.Services;
using Core.Domain.Entities;
using Xunit;

namespace Specs.StepDefinitions;

[Binding]
public sealed class CreateListingStepDefinitions
{
    private IListingService listingService;
    private IFileValidationService fileValidationService;
    private Mock<IListingRepo> mockListingRepo;
    private Mock<ICloudinaryImageService> mockCloudinary;
    private ListingCreateRequestDto listingCreateRequestDto;
    private ListingResponseDto createdListingResponseDto;
    private const string TestUserId = "user-1";

    
    public CreateListingStepDefinitions(ITestOutputHelper output)
    {
        mockListingRepo = new Mock<IListingRepo>();
        mockCloudinary = new Mock<ICloudinaryImageService>();
        fileValidationService = new FileValidationService();

        mockListingRepo
            .Setup(r => r.CreateListingAsync(It.IsAny<Listing>()))
            .ReturnsAsync((Listing l) =>
            {
                l.CreatedAt = DateTime.UtcNow;
                l.UpdatedAt = DateTime.UtcNow;
                l.Images = l.Images;
                return l;
            });

        mockCloudinary
            .Setup(c => c.UploadImageAsync(It.IsAny<System.IO.Stream>(), It.IsAny<string>()))
            .ReturnsAsync(new CloudinaryUploadResponseDto
            {
                PublicId = "public-id",
                SecureUrl = "https://example.com/image.jpg"
            });

        listingService = new ListingService(mockListingRepo.Object, mockCloudinary.Object, fileValidationService);
    }

    [Given("a listing create request with title (.*), categoryId (.*), condition (.*), description (.*), price (.*), status (.*)")]
    public void GivenAListingCreateRequest(string title, string categoryId, string condition, string description, decimal price, string status)
    {
        listingCreateRequestDto = new ListingCreateRequestDto
        {
            CategoryId = categoryId,
            Condition = condition,
            Title = title,
            Description = description,
            Price = price,
            Status = status,
            Images = new List<IFormFile>()
        };
    }

    [When("the user creates the listing")]
    public async Task WhenTheUserCreatesTheListing()
    {
        createdListingResponseDto = await listingService.CreateListingAsync(listingCreateRequestDto, TestUserId);
    }

    [Then("the listing should be created successfully")]
    public void ThenTheListingShouldBeCreatedSuccessfully()
    {
        Assert.NotNull(createdListingResponseDto);
        Assert.Equal(TestUserId, createdListingResponseDto.UserId);
        Assert.Equal(listingCreateRequestDto.Title, createdListingResponseDto.Title);

        mockListingRepo.Verify(r => r.CreateListingAsync(It.IsAny<Listing>()), Times.Once);
    }
}