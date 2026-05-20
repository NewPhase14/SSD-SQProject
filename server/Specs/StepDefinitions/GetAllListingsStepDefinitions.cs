using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
public sealed class GetAllListingsStepDefinitions
{
    private IListingService listingService;
    private Mock<IListingRepo> mockListingRepo;
    private Mock<ICloudinaryImageService> mockCloudinary;
    private IFileValidationService fileValidationService;
    private List<ListingResponseDto> responseList;
    private Exception capturedException;

    public GetAllListingsStepDefinitions()
    {
        mockListingRepo = new Mock<IListingRepo>();
        mockCloudinary = new Mock<ICloudinaryImageService>();
        fileValidationService = new FileValidationService();

        listingService = new ListingService(mockListingRepo.Object, mockCloudinary.Object, fileValidationService);
    }

    [Given("the repository has the following listings:")]
    public void GivenRepositoryHasListings(Table table)
    {
        var listings = new List<Listing>();
        foreach (var row in table.Rows)
        {
            listings.Add(new Listing
            {
                Id = row["id"],
                UserId = row["userId"],
                CategoryId = row["categoryId"],
                Condition = row["condition"],
                Title = row["title"],
                Description = row["description"],
                Price = decimal.Parse(row["price"]),
                Status = row["status"],
                Images = new List<Image>()
            });
        }

        mockListingRepo.Setup(r => r.GetAllListingsAsync()).ReturnsAsync(listings);
    }

    [Given("the repository has no listings")]
    public void GivenRepositoryHasNoListings()
    {
        mockListingRepo.Setup(r => r.GetAllListingsAsync()).ReturnsAsync(new List<Listing>());
    }

    [When("the user requests all listings")]
    public async Task WhenUserRequestsAllListings()
    {
        try
        {
            responseList = await listingService.GetAllListingsAsync();
        } catch (InvalidOperationException ex)        {
            capturedException = ex;
        }
    }

    [Then(@"the user should receive (.*) listings")]
    public void ThenUserShouldReceiveListings(int expectedCount)
    {
        Assert.Null(capturedException);
        Assert.NotNull(responseList);
        Assert.Equal(expectedCount, responseList.Count);
    }

    [Then(@"the user should see ""(.*)""")]
    public void ThenUserShouldSee(string message)
    {
        Assert.NotNull(capturedException);
        Assert.IsType<InvalidOperationException>(capturedException);
        Assert.Equal(message, capturedException.Message);
    }
}