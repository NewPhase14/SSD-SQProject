using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Conversations;
using Application.Services;
using Core.Domain.Entities;
using Moq;

namespace UnitTests.ConversationTests;

public class ConversationServiceTests
{
    private readonly IConversationService _conversationService;

    private readonly Mock<IConversationRepo> _mockConversationRepo = new();
    private readonly Mock<IListingRepo> _mockListingRepo = new();

    private static readonly User Buyer = new()
    {
        Id = "buyer-123",
        Name = "Morten Mortensen",
        Email = "morten@gmail.com",
    };

    private static readonly User Seller = new()
    {
        Id = "seller-456",
        Name = "Hans Hansen",
        Email = "hans@gmail.com",
    };

    private static readonly Listing Listing = new()
    {
        Id = "listing-001",
        UserId = Seller.Id,
        Title = "Laptop"
    };

    private static readonly Conversation ExistingConversation = new()
    {
        Id = "conv-001",
        ListingId = Listing.Id,
        BuyerUserId = Buyer.Id
    };
    
    public ConversationServiceTests()
    {
        _mockListingRepo
            .Setup(r => r.GetSellerIdAsync(Listing.Id))
            .ReturnsAsync(Seller.Id);

        _conversationService = new ConversationService(
            _mockConversationRepo.Object,
            _mockListingRepo.Object);
    }
    
    [Fact]
    public async Task GetOrCreateConversationAsync_ReturnsExistingConversation()
    {
        _mockConversationRepo
            .Setup(r => r.GetByListingAndBuyerAsync(Listing.Id, Buyer.Id))
            .ReturnsAsync(ExistingConversation);

        var dto = new ConversationCreateRequestDto
        {
            ListingId = Listing.Id
        };

        var result = await _conversationService
            .GetOrCreateConversationAsync(dto, Buyer.Id);

        Assert.NotNull(result);
        Assert.Equal(ExistingConversation.Id, result.Id);
        Assert.Equal(Buyer.Id, result.BuyerUserId);
        Assert.Equal(Seller.Id, result.SellerUserId);
    }

    [Fact]
    public async Task GetOrCreateConversationAsync_CreatesNewConversation()
    {
        _mockConversationRepo
            .Setup(r => r.GetByListingAndBuyerAsync(Listing.Id, Buyer.Id))
            .ReturnsAsync((Conversation?)null);

        _mockConversationRepo
            .Setup(r => r.CreateAsync(It.IsAny<Conversation>()))
            .ReturnsAsync((Conversation c) => c);

        var dto = new ConversationCreateRequestDto
        {
            ListingId = Listing.Id
        };

        var result = await _conversationService
            .GetOrCreateConversationAsync(dto, Buyer.Id);

        Assert.NotNull(result);
        Assert.Equal(Listing.Id, result.ListingId);
        Assert.Equal(Buyer.Id, result.BuyerUserId);
        Assert.Equal(Seller.Id, result.SellerUserId);

        _mockConversationRepo.Verify(
            r => r.CreateAsync(It.IsAny<Conversation>()),
            Times.Once);
    }

    [Fact]
    public async Task GetOrCreateConversationAsync_ListingNotFound_ThrowsException()
    {
        _mockListingRepo
            .Setup(r => r.GetSellerIdAsync(Listing.Id))
            .ReturnsAsync((string?)null);

        var dto = new ConversationCreateRequestDto
        {
            ListingId = Listing.Id
        };

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _conversationService.GetOrCreateConversationAsync(dto, Buyer.Id));

        Assert.Equal("Listing not found", ex.Message);
    }

    [Fact]
    public async Task GetOrCreateConversationAsync_OwnListing_ThrowsInvalidOperationException()
    {
        var dto = new ConversationCreateRequestDto
        {
            ListingId = Listing.Id
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _conversationService.GetOrCreateConversationAsync(dto, Seller.Id));

        Assert.Equal("Cannot create conversation with own listing", ex.Message);
    }
}
