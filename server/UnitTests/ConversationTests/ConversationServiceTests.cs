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
        // Arrange — conversation already exists, no new one should be created
        _mockConversationRepo
            .Setup(r => r.GetConversationByListingAndBuyerAsync(Listing.Id, Buyer.Id))
            .ReturnsAsync(ExistingConversation);
    
        // Act
        var result = await _conversationService
            .GetOrCreateConversationAsync(new ConversationCreateRequestDto { ListingId = Listing.Id }, Buyer.Id);
    
        // Assert
        Assert.NotNull(result);
        Assert.Equal(ExistingConversation.Id, result.Id);
        Assert.Equal(Buyer.Id, result.BuyerUserId);
        Assert.Equal(Seller.Id, result.SellerUserId);
    }
    
    [Fact]
    public async Task GetOrCreateConversationAsync_CreatesNewConversation()
    {
        // Arrange — no existing conversation, so a new one should be created
        _mockConversationRepo
            .Setup(r => r.GetConversationByListingAndBuyerAsync(Listing.Id, Buyer.Id))
            .ReturnsAsync((Conversation?)null);
    
        _mockConversationRepo
            .Setup(r => r.CreateConversationAsync(It.IsAny<Conversation>()))
            .ReturnsAsync((Conversation c) => c);
    
        // Act
        var result = await _conversationService
            .GetOrCreateConversationAsync(new ConversationCreateRequestDto { ListingId = Listing.Id }, Buyer.Id);
    
        // Assert
        Assert.NotNull(result);
        Assert.Equal(Listing.Id, result.ListingId);
        Assert.Equal(Buyer.Id, result.BuyerUserId);
        Assert.Equal(Seller.Id, result.SellerUserId);
    
        _mockConversationRepo.Verify(r => r.CreateConversationAsync(It.IsAny<Conversation>()), Times.Once);
    }
    
    [Fact]
    public async Task GetOrCreateConversationAsync_ListingNotFound_ThrowsException()
    {
        // Arrange
        _mockListingRepo
            .Setup(r => r.GetSellerIdAsync(Listing.Id))
            .ReturnsAsync((string?)null);
    
        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _conversationService.GetOrCreateConversationAsync(
                new ConversationCreateRequestDto { ListingId = Listing.Id }, Buyer.Id));
    
        Assert.Equal("Listing not found", ex.Message);
    }
    
    [Fact]
    public async Task GetOrCreateConversationAsync_OwnListing_ThrowsInvalidOperationException()
    {
        // Arrange & Act & Assert — a seller must not be able to message themselves
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _conversationService.GetOrCreateConversationAsync(
                new ConversationCreateRequestDto { ListingId = Listing.Id }, Seller.Id));
    
        Assert.Equal("Cannot create conversation with own listing", ex.Message);
    }
}
