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
        };
        
        var createdListing = await listingRepository.CreateListing(listing);
        
        var images = dto.ImagePaths.Select(path => new Image
        {
            Id = Guid.NewGuid().ToString(),
            ListingId = createdListing.Id,
            ImagePath = path
        }).ToList();
        
        var createdImages = await listingRepository.AddImages(images);
        
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
            ImagePaths = createdImages.Select(i => i.ImagePath).ToList(),
            CreatedAt = createdListing.CreatedAt,
            UpdatedAt = createdListing.UpdatedAt,
        };
    }

    public async Task<ListingResponseDto> UpdateListing(ListingUpdateRequestDto dto)
    {
        var listing = new Listing()
        {
            Id = dto.Id,
            CategoryId = dto.CategoryId,
            Condition = dto.Condition,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Status = dto.Status,
            UpdatedAt = DateTime.UtcNow,
        };
        
        var updatedListing = await listingRepository.UpdateListing(listing);
        return new ListingResponseDto()
        {
            Id = updatedListing.Id,
            UserId = updatedListing.UserId,
            CategoryId = updatedListing.CategoryId,
            Condition = updatedListing.Condition,
            Title = updatedListing.Title,
            Description = updatedListing.Description,
            Price = updatedListing.Price,
            Status = updatedListing.Status,
            CreatedAt = updatedListing.CreatedAt,
            UpdatedAt = updatedListing.UpdatedAt,
        };
    }

    public async Task<List<ListingResponseDto>> GetAllListings()
    {
        var  listings = await listingRepository.GetAllListings();
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
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt,
        }).ToList();
    }

    public async Task<List<ListingResponseDto>> GetListingsByUserId(string id)
    {
        var listings = await listingRepository.GetListingByUserId(id);
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
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt,
        }).ToList();
    }

    public async Task<ListingResponseDto> DeleteListing(string id)
    {
       var deletedListing = await listingRepository.DeleteListing(id);
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
           CreatedAt = deletedListing.CreatedAt,
           UpdatedAt = deletedListing.UpdatedAt,
       };
    }
}