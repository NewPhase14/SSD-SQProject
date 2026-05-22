using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models;
using Application.Models.Crypto;
using Application.Models.Dtos.Messages;
using Core.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class MessageService(IOptionsMonitor<Encryption> optionsMonitor, IMessageRepo messageRepo, IConversationRepo conversationRepo, IListingRepo listingRepo, ICryptoService cryptoService) : IMessageService
{
    
    public async Task<MessageResponseDto> SendMessageAsync(MessageSendRequestDto dto, string userId)
    {
        var conversation = await conversationRepo.GetConversationAsync(dto.ConversationId);

        if (conversation == null)
            throw new Exception("Conversation not found");
        
        var sellerId = await listingRepo.GetSellerIdAsync(conversation.ListingId);
        
        if (sellerId == null)
            throw new Exception("Listing not found");
        
        if (userId != conversation.BuyerUserId && userId != sellerId)
            throw new UnauthorizedAccessException("You are not a participant in this conversation");
        
        var encryptedText = cryptoService.Encrypt(dto.PlainText, GetEncryptionKey());
        
        var message = new Message
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = dto.ConversationId,
            SenderUserId = userId,
            Ciphertext = encryptedText.CipherText,
            Nonce = encryptedText.Nonce,
            Tag = encryptedText.Tag,
        };
        
        var sendMessage = await messageRepo.AddMessageAsync(message);
        
        return new MessageResponseDto()
        {
            Id = sendMessage.Id,
            ConversationId = sendMessage.ConversationId,
            SenderUserId = sendMessage.SenderUserId,
            Text = cryptoService.DecryptString(new EncryptedMessage(sendMessage.Ciphertext, sendMessage.Nonce, sendMessage.Tag), GetEncryptionKey()),
            CreatedAt = sendMessage.CreatedAt
        };
    }

    public async Task<List<MessageResponseDto>> GetMessagesAsync(string conversationId, string userId)
    {
        var conversation = await conversationRepo.GetConversationAsync(conversationId);
        
        if (conversation == null)
            throw new Exception("Conversation not found");
        
        var sellerId = await listingRepo.GetSellerIdAsync(conversation.ListingId);
        
        if (sellerId == null)
            throw new Exception("Listing not found");
        
        if (userId != conversation.BuyerUserId && userId != sellerId)
            throw new UnauthorizedAccessException("You are not a participant in this conversation");
        
        var messages = await messageRepo.GetByConversationIdAsync(conversationId);
        
        var response = messages.Select(m => new MessageResponseDto
        {
            Id = m.Id,
            ConversationId = m.ConversationId,
            SenderUserId = m.SenderUserId, 
            Text = cryptoService.DecryptString(new EncryptedMessage(m.Ciphertext, m.Nonce, m.Tag), GetEncryptionKey()),
            CreatedAt = m.CreatedAt
        }).ToList();
        
        return response;
    }
    
    private byte[] GetEncryptionKey()
    {
        return Convert.FromBase64String(
            optionsMonitor.CurrentValue.Key);
    }
}