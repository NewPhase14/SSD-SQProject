using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Conversation
{
    public string Id { get; set; } = null!;

    public string ListingId { get; set; } = null!;

    public string BuyerUserId { get; set; } = null!;

    public string SellerUserId { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual User BuyerUser { get; set; } = null!;

    public virtual Listing Listing { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual User SellerUser { get; set; } = null!;
}
