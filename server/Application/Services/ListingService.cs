using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Listings;
using Core.Domain.Entities;

namespace Application.Services;

public class ListingService(IListingRepo listingRepo) : IListingService
{
    
    public async Task<ListingResponseDto> CreateListingAsync(ListingCreateRequestDto dto, string userId)
    {
        var listingId = Guid.NewGuid().ToString();
        var listing = new Listing()
        {
            Id = listingId,
            UserId = userId,
            CategoryId = dto.CategoryId,
            Condition = dto.Condition,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Status = dto.Status,
            Images = dto.ImagePaths.Select(p => new Image()
            {
                Id = Guid.NewGuid().ToString(),
                ListingId = listingId,
                ImagePath = p,
            }).ToList(),
        };
        
        var createdListing = await listingRepo.CreateListingAsync(listing);
        
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
            ImagePaths = createdListing.Images.Select(i => i.ImagePath).ToList(),
            CreatedAt = createdListing.CreatedAt,
            UpdatedAt = createdListing.UpdatedAt,
        };
    }

    public async Task<ListingResponseDto> UpdateListingAsync(ListingUpdateRequestDto dto, string userId)
    {
        var sellerId  = await listingRepo.GetSellerIdAsync(dto.Id);
        
        if (sellerId == null)
            throw new InvalidOperationException("Listing not found");
        
        if (sellerId != userId)
            throw new UnauthorizedAccessException("You are not the owner of this listing");
        
        var listing = new Listing()
        {
            Id = dto.Id,
            UserId = userId,
            CategoryId = dto.CategoryId,
            Condition = dto.Condition,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Status = dto.Status,
            UpdatedAt = DateTime.UtcNow,
        };
        
        var updatedListing = await listingRepo.UpdateListingAsync(listing);
        if (updatedListing == null)
            throw new InvalidOperationException("Listing not found");
        
        return new ListingResponseDto()
        {
            Id = updatedListing.Id,
            UserId = updatedListing.UserId,
            CategoryId = updatedListing.CategoryId,
            Condition = updatedListing.Condition,
            Title = updatedListing.Title,
            Description = updatedListing.Description,
            ImagePaths = updatedListing.Images.Select(i => i.ImagePath).ToList(),
            Price = updatedListing.Price,
            Status = updatedListing.Status,
            CreatedAt = updatedListing.CreatedAt,
            UpdatedAt = updatedListing.UpdatedAt,
        };
    }

    public async Task<List<ListingResponseDto>> GetAllListingsAsync()
    {
        var  listings = await listingRepo.GetAllListingsAsync();
        if (listings.Count == 0)
            throw new InvalidOperationException("No listings found");
        
        return listings.Select(l => new ListingResponseDto()
        {
            Id = l.Id,
            UserId = l.UserId,
            CategoryId = l.CategoryId,
            Condition = l.Condition,
            Title = l.Title,
            Description = l.Description,
            Price = l.Price,
            Status = l.Status,
            ImagePaths = l.Images.Select(i => i.ImagePath).ToList(),
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt,
        }).ToList();
    }

    public async Task<List<ListingResponseDto>> GetListingsByUserIdAsync(string id)
    {
        var listings = await listingRepo.GetListingByUserIdAsync(id);
        return listings.Select(l => new ListingResponseDto()
        {
            Id = l.Id,
            UserId = l.UserId,
            CategoryId = l.CategoryId,
            Condition = l.Condition,
            Title = l.Title,
            Description = l.Description,
            Price = l.Price,
            Status = l.Status,
            ImagePaths = l.Images.Select(i => i.ImagePath).ToList(),
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt
        }).ToList();
    }

    public async Task<ListingResponseDto> DeleteListingAsync(string listingId,  string userId)
    {
        var sellerId  = await listingRepo.GetSellerIdAsync(listingId);
        
        if (sellerId == null)
            throw new InvalidOperationException("Listing not found");
        
        if (sellerId != userId)
            throw new UnauthorizedAccessException("You are not the owner of this listing");
        
        var deletedListing = await listingRepo.DeleteListingAsync(listingId); 
        
        if (deletedListing == null)
            throw new InvalidOperationException("Listing not found");
        
        return new ListingResponseDto 
        { 
            Id = deletedListing.Id, 
            UserId = deletedListing.UserId, 
            CategoryId = deletedListing.CategoryId, 
            Condition = deletedListing.Condition, 
            Title = deletedListing.Title, 
            Description = deletedListing.Description, 
            Price = deletedListing.Price, 
            Status = deletedListing.Status, 
            ImagePaths = deletedListing.Images.Select(i => i.ImagePath).ToList(),
            CreatedAt = deletedListing.CreatedAt, 
            UpdatedAt = deletedListing.UpdatedAt, 
        }; 
    }
}