using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Category
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
