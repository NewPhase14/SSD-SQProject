using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Message
{
    public string Id { get; set; } = null!;

    public string ConversationId { get; set; } = null!;

    public string SenderUserId { get; set; } = null!;

    public byte[] Ciphertext { get; set; } = null!;

    public byte[] Nonce { get; set; } = null!;

    public byte[] Tag { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User SenderUser { get; set; } = null!;
}
