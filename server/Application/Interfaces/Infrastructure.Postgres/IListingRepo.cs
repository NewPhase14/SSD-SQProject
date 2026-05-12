using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IListingRepo
{
    Task<Listing> CreateListingAsync(Listing listing);
    Task<Listing?> UpdateListingAsync(Listing listing);
    Task<List<Listing>> GetAllListingsAsync();
    Task<List<Listing>> GetListingByUserIdAsync(string id);
    Task<Listing?> DeleteListingAsync(string id);
    Task<string?> GetSellerIdAsync(string listingId);
}