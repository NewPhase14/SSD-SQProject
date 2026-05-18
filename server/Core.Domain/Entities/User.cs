using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class User
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public byte[]? TfaSecret { get; set; }

    public byte[]? Nonce { get; set; }

    public byte[]? Tag { get; set; }

    public bool IsTfaEnabled { get; set; }

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
