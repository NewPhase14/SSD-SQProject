using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos;
using Core.Domain.Entities;

namespace Application.Services;

public class ListingService(IListingRepository listingRepository) : IListingService
{
    
    public async Task<ListingResponseDto> CreateListing(ListingCreateRequestDto dto)
    {
        var listing = new Listing()
        {
            Id = Guid.NewGuid().ToString(),
            UserId = dto.UserId,
            CategoryId = dto.CategoryId,
            Condition = dto.Condition,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Status = dto.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        
        var createdListing = await listingRepository.CreateListing(listing);
        
        return new ListingResponseDto()
        {
            Id = createdListing.Id,
            UserId = createdListing.UserId,
            CategoryId = createdListing.CategoryId,
            Condition = createdListing.Condition,
            Title = createdListing.Title,
            Description = createdListing.Description,
            Price = createdListing.Price,
            Status = createdListing.Status,
            CreatedAt = createdListing.CreatedAt,
            UpdatedAt = createdListing.UpdatedAt,
        };
    }

    public Task<ListingResponseDto> UpdateListing(ListingUpdateRequestDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<List<Listing>> GetAllListings()
    {
        throw new NotImplementedException();
    }

    public Task<ListingResponseDto> GetListingByUserId(string id)
    {
        throw new NotImplementedException();
    }

    public Task<ListingResponseDto> DeleteListing(string id)
    {
        throw new NotImplementedException();
    }
}