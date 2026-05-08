using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Image
{
    public string Id { get; set; } = null!;

    public string ImagePath { get; set; } = null!;

    public string ListingId { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Listing Listing { get; set; } = null!;
}
