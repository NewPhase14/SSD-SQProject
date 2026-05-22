using Application.Models.Dtos.Listings;

namespace Application.Interfaces;

public interface IListingService
{
    public Task<ListingResponseDto> CreateListingAsync(ListingCreateRequestDto dto, string userId);
    
    public Task<ListingResponseDto> UpdateListingAsync(ListingUpdateRequestDto dto, string userId);
    
    public Task<List<ListingResponseDto>> GetAllListingsAsync();
    
    public Task<List<ListingResponseDto>> GetListingsByUserIdAsync(string userId);
    
    public Task<ListingResponseDto> GetListingByIdAsync(string listingId);
    
    public Task<ListingResponseDto> DeleteListingAsync(string listingId, string userId);
}