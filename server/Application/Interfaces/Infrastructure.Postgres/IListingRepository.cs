using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IListingRepository
{
    Task<Listing> CreateListing(Listing listing);
    Task<Listing> UpdateListing(Listing listing);
    Task<List<Listing>> GetAllListings();
    Task<List<Listing>> GetListingByUserId(string id);
    Task<Listing> DeleteListing(string id);
    Task<List<Image>> AddImages(List<Image> images);
}