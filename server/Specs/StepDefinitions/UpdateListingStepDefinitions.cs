using System;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Listings;
using Moq;
using Application.Services;
using Core.Domain.Entities;
using Reqnroll;
using Xunit;

namespace Specs.StepDefinitions;

[Binding]
public sealed class UpdateListingStepDefinitions
{
    private IListingService listingService;
    private Mock<IListingRepo> mockListingRepo;
    private Mock<ICloudinaryImageService> mockCloudinary;
    private IFileValidationService fileValidationService;
    private ListingUpdateRequestDto updateDto;
    private ListingResponseDto updatedResponse;
    private Exception capturedException;

    public UpdateListingStepDefinitions()
    {
        mockListingRepo = new Mock<IListingRepo>();
        mockCloudinary = new Mock<ICloudinaryImageService>();
        fileValidationService = new FileValidationService();

        mockListingRepo
            .Setup(r => r.UpdateListingAsync(It.IsAny<Listing>()))
            .ReturnsAsync((Listing l) =>
            {
                l.UpdatedAt = DateTime.UtcNow;
                l.CreatedAt = DateTime.UtcNow.AddMinutes(-5);
                return l;
            });

        listingService = new ListingService(mockListingRepo.Object, mockCloudinary.Object, fileValidationService);
    }

    [Given(@"an existing listing with id ""(.*)"" owned by ""(.*)""")]
    public void GivenAnExistingListing(string id, string ownerId)
    {
        mockListingRepo
            .Setup(r => r.GetSellerIdAsync(id))
            .ReturnsAsync(ownerId);

        mockListingRepo
            .Setup(r => r.UpdateListingAsync(It.Is<Listing>(l => l.Id == id)))
            .ReturnsAsync((Listing l) =>
            {
                l.CreatedAt = DateTime.UtcNow.AddDays(-1);
                l.UpdatedAt = DateTime.UtcNow;
                return l;
            });
    }

    [Given(@"no listing exists with id ""(.*)""")]
    public void GivenNoListingExists(string id)
    {
        mockListingRepo
            .Setup(r => r.GetSellerIdAsync(id))
            .ReturnsAsync((string)null);
    }

    [Given("an update request:")]
    public void GivenAnUpdateRequest(Table table)
    {
        updateDto = new ListingUpdateRequestDto
        {
            Id = table.Rows[0]["id"],
            CategoryId = table.Rows[0]["categoryId"],
            Condition = table.Rows[0]["condition"],
            Title = table.Rows[0]["title"],
            Description = table.Rows[0]["description"],
            Price = decimal.Parse(table.Rows[0]["price"]),
            Status = table.Rows[0]["status"]
        };
    }

    [When(@"the user ""(.*)"" updates the listing")]
    public void WhenTheUserUpdatesTheListing(string userId)
    {
        try
        {
            updatedResponse = listingService.UpdateListingAsync(updateDto, userId).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            capturedException = ex;
        }
    }

    [Then("the listing should be updated successfully")]
    public void ThenListingUpdatedSuccessfully()
    {
        Assert.Null(capturedException);
        Assert.NotNull(updatedResponse);
        Assert.Equal(updateDto.Title, updatedResponse.Title);
        Assert.Equal(updateDto.Description, updatedResponse.Description);
        mockListingRepo.Verify(r => r.GetSellerIdAsync(updateDto.Id), Times.Once);
        mockListingRepo.Verify(r => r.UpdateListingAsync(It.IsAny<Listing>()), Times.Once);
    }

    [Then(@"the user should be denied with ""(.*)""")]
    public void ThenTheUserShouldBeDeniedWith(string expectedMessage)
    {
        Assert.NotNull(capturedException);
        Assert.Equal(expectedMessage, capturedException.Message);
    }

    [Then(@"the user should see an update error ""(.*)""")]
    public void ThenTheUserShouldSeeUpdateError(string expectedMessage)
    {
        Assert.NotNull(capturedException);
        Assert.Equal(expectedMessage, capturedException.Message);
    }
}