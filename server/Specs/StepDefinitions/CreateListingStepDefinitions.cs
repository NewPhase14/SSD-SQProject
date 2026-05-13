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
    private readonly ITestOutputHelper _output;
    private IListingService listingService;
    private IFileValidationService fileValidationService;
    private Mock<IListingRepo> mockListingRepo;
    private Mock<ICloudinaryImageService> mockCloudinary;
    private ListingCreateRequestDto listingCreateRequestDto;
    private ListingResponseDto createdListingResponseDto;
    private const string TestUserId = "lkfjsdlkfjs1234";

    
    public CreateListingStepDefinitions(ITestOutputHelper output)
    {
        _output = output;

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

    [Given("a listing create request:")]
    public void GivenAListingCreateRequest(Table table)
    {
        listingCreateRequestDto = new ListingCreateRequestDto
        {
            CategoryId = table.Rows[0]["categoryId"],
            Condition = table.Rows[0]["condition"],
            Title = table.Rows[0]["title"],
            Description = table.Rows[0]["description"],
            Price = decimal.Parse(table.Rows[0]["price"]),
            Status = table.Rows[0]["status"],
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
        Assert.Equal(listingCreateRequestDto.Condition, createdListingResponseDto.Condition);
        Assert.Equal(listingCreateRequestDto.Title, createdListingResponseDto.Title);
        Assert.Equal(listingCreateRequestDto.Description, createdListingResponseDto.Description);
        Assert.Equal(listingCreateRequestDto.Price, createdListingResponseDto.Price);
        Assert.Equal(listingCreateRequestDto.Status, createdListingResponseDto.Status);
        Assert.Equal(TestUserId, createdListingResponseDto.UserId);
        Assert.Equal(listingCreateRequestDto.Images.Count, createdListingResponseDto.ImageUrls.Count);
        
        mockListingRepo.Verify(r => r.CreateListingAsync(It.IsAny<Listing>()), Times.Exactly(1));
        _output.WriteLine($"Images.Count = {listingCreateRequestDto.Images.Count}");
        _output.WriteLine($"ImageUrls.Count = {createdListingResponseDto.ImageUrls.Count}");
        _output.WriteLine(createdListingResponseDto.CreatedAt.ToString());
    }
}