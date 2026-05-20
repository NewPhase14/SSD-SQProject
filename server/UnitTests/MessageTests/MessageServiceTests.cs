using System.Security.Cryptography;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models;
using Application.Models.Dtos.Messages;
using Application.Services;
using Core.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;

namespace UnitTests.MessageTests;

public class MessageServiceTests
{
    private readonly IMessageService _messageService;
    private readonly Mock<IMessageRepo> _mockMessageRepo = new();
    private readonly Mock<IConversationRepo> _mockConversationRepo = new();
    private readonly Mock<IListingRepo> _mockListingRepo = new();
    private readonly ICryptoService _cryptoService = new CryptoService();
    
    // Master-key
    private static IOptionsMonitor<Encryption> EncryptionOptions() =>
        Mock.Of<IOptionsMonitor<Encryption>>(m =>
            m.CurrentValue == new Encryption { Key = "bm9uZWJpZ2dlc3Rib29rZ2VuZXJhbHJvc2V0aG91c2F=" });
    
    private static readonly User Buyer = new()
    {
        Id           = "buyer-123",
        Name         = "Morten Mortensen",
        Email        = "morten@gmail.com",
    };
 
    private static readonly User Seller = new()
    {
        Id           = "seller-456",
        Name         = "Hans Hansen",
        Email        = "hans@gmail.com",
    };
 
    private static readonly User Attacker = new()
    {
        Id           = "outsider-789",
        Name         = "Ole Olesen",
        Email        = "ole@gmail.com",
    };
 
    private static readonly Category Category = new()
    {
        Id   = "cat-001",
        Name = "Electronics"
    };
 
    private static readonly Listing Listing = new()
    {
        Id          = "listing-001",
        UserId      = Seller.Id,
        CategoryId  = Category.Id,
        Condition   = "Used",
        Title       = "Old Laptop",
        Description = "Works fine",
        Price       = 300,
        Status      = "Active",
        Category    = Category,
        User        = Seller
    };
 
    private static readonly Conversation Conversation = new()
    {
        Id          = "conv-001",
        ListingId   = Listing.Id,
        BuyerUserId = Buyer.Id,
        BuyerUser   = Buyer,
        Listing     = Listing
    };
    
    public MessageServiceTests()
    {
        // Setup mocks for repositories
        _mockConversationRepo
            .Setup(r => r.GetAsync(Conversation.Id))
            .ReturnsAsync(Conversation);
 
        _mockListingRepo
            .Setup(r => r.GetSellerIdAsync(Listing.Id))
            .ReturnsAsync(Seller.Id);
 
        _mockMessageRepo
            .Setup(r => r.AddMessageAsync(It.IsAny<Message>()))
            .ReturnsAsync((Message m) => m);
        
        _messageService = new MessageService(
            EncryptionOptions(),
            _mockMessageRepo.Object,
            _mockConversationRepo.Object,
            _mockListingRepo.Object,
            _cryptoService);
    }
    
    [Fact]
    public async Task SendMessageAsync_AsBuyer_ReturnsDecryptedMessage()
    {
        var dto = new MessageSendRequestDto { ConversationId = Conversation.Id, PlainText = "Hello!" };
 
        var result = await _messageService.SendMessageAsync(dto, Buyer.Id);
 
        Assert.Equal("Hello!", result.Text);
        Assert.Equal(Conversation.Id, result.ConversationId);
        Assert.Equal(Buyer.Id, result.SenderUserId);
    }
    
    [Fact]
    public async Task SendMessageAsync_AsSeller_ReturnsDecryptedMessage()
    {
        var dto = new MessageSendRequestDto { ConversationId = Conversation.Id, PlainText = "Hi buyer!" };
 
        var result = await _messageService.SendMessageAsync(dto, Seller.Id);
 
        Assert.NotNull(result);
        Assert.Equal("Hi buyer!", result.Text);
        Assert.Equal(Seller.Id, result.SenderUserId);
    }
    
    [Fact]
    public async Task SendMessageAsync_StoredCiphertext_IsNotPlaintext()
    {
        const string plainText = "Hello, this is a secret message!";
        var dto = new MessageSendRequestDto { ConversationId = Conversation.Id, PlainText = plainText };
        
        // Capture the message being added to the repository
        Message? capturedMessage = null;
        _mockMessageRepo.Setup(r => r.AddMessageAsync(It.IsAny<Message>()))
            .ReturnsAsync((Message m) =>
            {
                capturedMessage = m;
                return m;
            }); 
        
        await _messageService.SendMessageAsync(dto, Buyer.Id);
        
        Assert.NotNull(capturedMessage);
        Assert.NotEqual(plainText, capturedMessage.Ciphertext.ToString());
    }
    
    [Fact]
    public async Task SendMessageAsync_ConversationNotFound_ThrowsException()
    {
        _mockConversationRepo
            .Setup(r => r.GetAsync(Conversation.Id))
            .ReturnsAsync((Conversation?)null);
 
        var dto = new MessageSendRequestDto { ConversationId = Conversation.Id, PlainText = "Hi" };
 
        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _messageService.SendMessageAsync(dto, Buyer.Id));
        Assert.Equal("Conversation not found", ex.Message);
    }
    
    [Fact]
    public async Task SendMessageAsync_ListingNotFound_ThrowsException()
    {
        _mockListingRepo
            .Setup(r => r.GetSellerIdAsync(Listing.Id))
            .ReturnsAsync((string?)null);
 
        var dto = new MessageSendRequestDto { ConversationId = Conversation.Id, PlainText = "Hi" };
 
        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _messageService.SendMessageAsync(dto, Buyer.Id));
        Assert.Equal("Listing not found", ex.Message);
    }
    
    [Fact]
    public async Task SendMessageAsync_RandomUser_ThrowsUnauthorizedAccessException()
    {
        var dto = new MessageSendRequestDto { ConversationId = Conversation.Id, PlainText = "Can i join this conversation?" };
 
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _messageService.SendMessageAsync(dto, Attacker.Id));
    }
    
    [Fact]
    public async Task GetMessagesAsync_ReturnsDecryptedMessages()
    {
        var key = Convert.FromBase64String(EncryptionOptions().CurrentValue.Key);

        var encrypted = _cryptoService.Encrypt("Secret message", key);

        var messages = new List<Message>
        {
            new()
            {
                Id = "msg-001",
                ConversationId = Conversation.Id,
                SenderUserId = Buyer.Id,
                Ciphertext = encrypted.CipherText,
                Nonce = encrypted.Nonce,
                Tag = encrypted.Tag,
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockMessageRepo
            .Setup(r => r.GetByConversationIdAsync(Conversation.Id))
            .ReturnsAsync(messages);

        var result = await _messageService.GetMessagesAsync(Conversation.Id, Buyer.Id);

        Assert.Equal("Secret message", result[0].Text);
        Assert.Equal(Buyer.Id, result[0].SenderUserId);
    }

    [Fact]
    public async Task GetMessagesAsync_ConversationNotFound_ThrowsException()
    {
        _mockConversationRepo
            .Setup(r => r.GetAsync(Conversation.Id))
            .ReturnsAsync((Conversation?)null);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _messageService.GetMessagesAsync(Conversation.Id, Buyer.Id));

        Assert.Equal("Conversation not found", ex.Message);
    }

    [Fact]
    public async Task GetMessagesAsync_ListingNotFound_ThrowsException()
    {
        _mockListingRepo
            .Setup(r => r.GetSellerIdAsync(Listing.Id))
            .ReturnsAsync((string?)null);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _messageService.GetMessagesAsync(Conversation.Id, Buyer.Id));

        Assert.Equal("Listing not found", ex.Message);
    }

    [Fact]
    public async Task GetMessagesAsync_RandomUser_ThrowsUnauthorizedAccessException()
    {
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _messageService.GetMessagesAsync(Conversation.Id, Attacker.Id));
    }
    
   
    
    
    
    
    
    
    
}