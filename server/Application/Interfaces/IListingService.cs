using Application.Models.Dtos.Listings;

namespace Application.Interfaces;

public interface IListingService
{
    Task<ListingResponseDto> CreateListingAsync(ListingCreateRequestDto dto, string userId);
    
    Task<ListingResponseDto> UpdateListingAsync(ListingUpdateRequestDto dto, string userId);
    
    Task<List<ListingResponseDto>> GetAllListingsAsync();
    
    Task<List<ListingResponseDto>> GetListingsByUserIdAsync(string userId);
    
    Task<ListingResponseDto> DeleteListingAsync(string listingId, string userId);
}