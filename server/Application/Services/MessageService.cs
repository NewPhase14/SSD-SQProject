using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models;
using Application.Models.Crypto;
using Application.Models.Dtos;
using Core.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class MessageService(IOptionsMonitor<Encryption> optionsMonitor, IMessageRepo messageRepo, IConversationRepo conversationRepo, IListingRepository listingRepository, ICryptoService cryptoService) : IMessageService
{
    
    public async Task SendMessageAsync(SendMessageRequestDto dto, string userId)
    {
        var conversation = await conversationRepo.GetAsync(dto.ConversationId);

        if (conversation == null)
            throw new Exception("Conversation not found");
        
        var sellerId = await listingRepository.GetSellerIdAsync(conversation.ListingId);
        
        if (sellerId == null)
            throw new Exception("Listing not found");
        
        if (userId != conversation.BuyerUserId && userId != sellerId)
            throw new UnauthorizedAccessException("You are not a participant in this conversation");
        
        var key = Convert.FromBase64String(optionsMonitor.CurrentValue.Key);
        
        var encryptedText = cryptoService.Encrypt(dto.PlainText, key);
        
        var message = new Message
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = dto.ConversationId,
            SenderUserId = userId,
            Ciphertext = encryptedText.CipherText,
            Nonce = encryptedText.Nonce,
            Tag = encryptedText.Tag,
        };
        
        await messageRepo.AddMessageAsync(message);
        
    }

    public async Task<List<MessageResponseDto>> GetMessagesAsync(string conversationId, string userId)
    {
        var conversation = await conversationRepo.GetAsync(conversationId);
        
        if (conversation == null)
            throw new Exception("Conversation not found");
        
        var sellerId = await listingRepository.GetSellerIdAsync(conversation.ListingId);
        
        if (sellerId == null)
            throw new Exception("Listing not found");
        
        if (userId != conversation.BuyerUserId && userId != sellerId)
            throw new UnauthorizedAccessException("You are not a participant in this conversation");
        
        var key = Convert.FromBase64String(optionsMonitor.CurrentValue.Key);
        
        var messages = await messageRepo.GetByConversationIdAsync(conversationId);
        
        var response = messages.Select(m => new MessageResponseDto
        {
            Id = m.Id,
            ConversationId = m.ConversationId,
            SenderUserId = m.SenderUserId, 
            Text = cryptoService.DecryptString(new EncryptedMessage(m.Ciphertext, m.Nonce, m.Tag), key),
            CreatedAt = m.CreatedAt
        }).ToList();
        
        return response;
    }
}