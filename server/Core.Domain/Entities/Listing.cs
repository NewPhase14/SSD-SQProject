using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Listing
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string CategoryId { get; set; } = null!;

    public string Condition { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual User User { get; set; } = null!;
}
