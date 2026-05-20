using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Listings;
using Application.Services;
using Core.Domain.Entities;
using Moq;
using Reqnroll;
using Xunit;

namespace Specs.StepDefinitions;

[Binding]
public sealed class DeleteListingStepDefinitions
{
    private string listingId;
    private string ownerUserId;
    private string actingUserId;

    private readonly Mock<IListingRepo> mockListingRepo;
    private readonly Mock<ICloudinaryImageService> mockCloudinary;
    private readonly IFileValidationService fileValidationService;
    private readonly IListingService listingService;

    private ListingResponseDto deletedResponse;
    private Exception capturedException;

    public DeleteListingStepDefinitions()
    {
        mockListingRepo = new Mock<IListingRepo>();
        mockCloudinary = new Mock<ICloudinaryImageService>();
        fileValidationService = new FileValidationService();

        listingService = new ListingService(
            mockListingRepo.Object,
            mockCloudinary.Object,
            fileValidationService);
    }

    [Given("the {string} with {string}")]
    public void GivenTheListingWithId(string listingIdValue, string userIdValue)
    {
        listingId = listingIdValue;
        ownerUserId = userIdValue;
        actingUserId = userIdValue;

        var listing = new Listing
        {
            Id = listingId,
            CategoryId = "cat-1",
            Condition = "New",
            UserId = ownerUserId,
            Title = "Test Listing",
            Description = "A listing for testing deletion.",
            Price = 100,
            Status = "Active",
        };

        mockListingRepo
            .Setup(r => r.GetListingByIdAsync(listingId))
            .ReturnsAsync(listing);

        mockListingRepo
            .Setup(r => r.DeleteListingAsync(listingId))
            .ReturnsAsync(listing);
        
        mockCloudinary
            .Setup(c => c.DeleteImagesAsync(It.IsAny<List<string>>()))
            .Returns(Task.CompletedTask);
    }

    [When("the user presses delete on listing")]
    public void WhenTheUserPressesDeleteOnListing()
    {
            deletedResponse = listingService.DeleteListingAsync(listingId, actingUserId).GetAwaiter().GetResult();
    }

    [When("a different user presses delete on listing")]
    public void WhenADifferentUserPressesDeleteOnListing()
    {
        actingUserId = "different-user";
        
        try
        {
            deletedResponse = listingService.DeleteListingAsync(listingId, actingUserId).GetAwaiter().GetResult();
        }
        catch (UnauthorizedAccessException ex)
        {
            capturedException = ex;
        }
    }

    [Then("the listing should be deleted successfully")]
    public void ThenTheListingShouldBeDeletedSuccessfully()
    {
        Assert.Null(capturedException);
        Assert.NotNull(deletedResponse);
        Assert.Equal(listingId, deletedResponse!.Id);
        Assert.Equal(ownerUserId, deletedResponse.UserId);

        mockListingRepo.Verify(r => r.GetListingByIdAsync(listingId), Times.Once);
        mockCloudinary.Verify(c => c.DeleteImagesAsync(It.IsAny<List<string>>()), Times.Once);
        mockListingRepo.Verify(r => r.DeleteListingAsync(listingId), Times.Once);
    }

    [Then("the delete should be rejected")]
    public void ThenTheDeleteShouldBeRejected()
    {
        Assert.Null(deletedResponse);
        Assert.NotNull(capturedException);
        Assert.IsType<UnauthorizedAccessException>(capturedException);

        mockListingRepo.Verify(r => r.GetListingByIdAsync(listingId), Times.Once);
        mockListingRepo.Verify(r => r.DeleteListingAsync(It.IsAny<string>()), Times.Never);
    }
}