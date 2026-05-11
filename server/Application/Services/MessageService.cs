using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models;
using Application.Models.Crypto;
using Application.Models.Dtos;
using Core.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class MessageService(IOptionsMonitor<Encryption> optionsMonitor, IMessageRepo messageRepo, IConversationRepo conversationRepo, ICryptoService cryptoService) : IMessageService
{
    
    public async Task SendMessageAsync(string conversationId, string senderUserId, string plainText)
    {
        var conversation = await conversationRepo.GetAsync(conversationId);

        if (conversation == null)
            throw new Exception("Conversation not found");
        
        var key = Convert.FromBase64String(optionsMonitor.CurrentValue.Key);
        
        var encryptedText = cryptoService.Encrypt(plainText, key);
        
        var message = new Message
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = conversationId,
            SenderUserId = senderUserId,
            Ciphertext = encryptedText.CipherText,
            Nonce = encryptedText.Nonce,
            Tag = encryptedText.Tag,
        };
        
        await messageRepo.AddMessageAsync(message);
        
    }

    public Task<List<MessageResponseDto>> GetMessagesAsync(string conversationId)
    {
        var conversation = conversationRepo.GetAsync(conversationId).Result;
        
        if (conversation == null)
            throw new Exception("Conversation not found");
        
        var key = Convert.FromBase64String(optionsMonitor.CurrentValue.Key);
        
        var messages = messageRepo.GetByConversationIdAsync(conversationId).Result;
        
        var response = messages.Select(m => new MessageResponseDto
        {
            Id = m.Id,
            ConversationId = m.ConversationId,
            SenderUserId = m.SenderUserId, 
            Text = cryptoService.DecryptString(new EncryptedMessage(m.Ciphertext, m.Nonce, m.Tag), key),
        }).ToList();
        
        return Task.FromResult(response);
    }
}