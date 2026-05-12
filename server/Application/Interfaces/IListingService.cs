using Application.Models.Dtos.Listings;

namespace Application.Interfaces;

public interface IListingService
{
    Task<ListingResponseDto> CreateListing(ListingCreateRequestDto dto, string userId);
    
    Task<ListingResponseDto> UpdateListing(ListingUpdateRequestDto dto, string userId);
    
    Task<List<ListingResponseDto>> GetAllListings();
    
    Task<List<ListingResponseDto>> GetListingsByUserId(string userId);
    
    Task<ListingResponseDto> DeleteListing(string listingId, string userId);
}