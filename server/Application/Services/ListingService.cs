using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Listings;
using Core.Domain.Entities;

namespace Application.Services;

public class ListingService(IListingRepo listingRepo) : IListingService
{
    
    public async Task<ListingResponseDto> CreateListing(ListingCreateRequestDto dto, string userId)
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
        
        var createdListing = await listingRepo.CreateListing(listing);
        
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

    public async Task<ListingResponseDto> UpdateListing(ListingUpdateRequestDto dto, string userId)
    {
        var sellerId  = await listingRepo.GetSellerIdAsync(dto.Id);
        
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
        
        var updatedListing = await listingRepo.UpdateListing(listing);
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

    public async Task<List<ListingResponseDto>> GetAllListings()
    {
        var  listings = await listingRepo.GetAllListings();
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

    public async Task<List<ListingResponseDto>> GetListingsByUserId(string id)
    {
        var listings = await listingRepo.GetListingByUserId(id);
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

    public async Task<ListingResponseDto> DeleteListing(string listingId,  string userId)
    {
        var sellerId  = await listingRepo.GetSellerIdAsync(listingId);
        
        if (sellerId != userId)
            throw new UnauthorizedAccessException("You are not the owner of this listing");
        
        var deletedListing = await listingRepo.DeleteListing(listingId); 
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