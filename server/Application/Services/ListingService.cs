using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Listings;
using Core.Domain.Entities;

namespace Application.Services;

public class ListingService(IListingRepo listingRepo, ICloudinaryImageService cloudinaryImageService, IFileValidationService fileValidationService) : IListingService
{
    
    public async Task<ListingResponseDto> CreateListingAsync(ListingCreateRequestDto dto, string userId)
    {
        var listingId = Guid.NewGuid().ToString();

        var images = new List<Image>();
        
        // Upload images to Cloudinary and get the URLs
        foreach (var file in dto.Images.Where(file => file.Length > 0))
        {
            await using var stream = file.OpenReadStream();

            var fileName = Guid.NewGuid().ToString();
            
            // Validate file before upload
            await fileValidationService.ValidateImageAsync(stream, file.FileName, file.ContentType);

            var uploadResult =
                await cloudinaryImageService.UploadImageAsync(stream, fileName);

            images.Add(new Image
            {
                Id = Guid.NewGuid().ToString(),
                ListingId = listingId,
                ImageUrl = uploadResult.SecureUrl,
                PublicId = uploadResult.PublicId
            });
        }
        
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
            Images = images,
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
            ImageUrls = createdListing.Images.Select(i => i.ImageUrl).ToList(),
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
            ImageUrls = updatedListing.Images.Select(i => i.ImageUrl).ToList(),
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
            ImageUrls = l.Images.Select(i => i.ImageUrl).ToList(),
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt,
        }).ToList();
    }

    public async Task<List<ListingResponseDto>> GetListingsByUserIdAsync(string userId)
    {
        var listings = await listingRepo.GetListingByUserIdAsync(userId);
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
            ImageUrls = l.Images.Select(i => i.ImageUrl).ToList(),            
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt
        }).ToList();
    }

    public async Task<ListingResponseDto> GetListingByIdAsync(string listingId)
    {
        var listing = await listingRepo.GetListingByIdAsync(listingId);
        
        if (listing == null)
            throw new InvalidOperationException("Listing not found");

        return new ListingResponseDto
        {
            Id = listing.Id,
            UserId = listing.UserId,
            CategoryId = listing.CategoryId,
            Condition = listing.Condition,
            Title = listing.Title,
            Description = listing.Description,
            Price = listing.Price,
            Status = listing.Status,
            ImageUrls = listing.Images.Select(i => i.ImageUrl).ToList(),
            CreatedAt = listing.CreatedAt,
            UpdatedAt = listing.UpdatedAt
        };
    }

    public async Task<ListingResponseDto> DeleteListingAsync(string listingId,  string userId)
    {
        var listing =
            await listingRepo.GetListingByIdAsync(listingId);

        if (listing == null)
            throw new InvalidOperationException("Listing not found");
        
        if (listing.UserId != userId)
            throw new UnauthorizedAccessException("You are not the owner of this listing");
        
        var publicIds = listing.Images.Select(i => i.PublicId).ToList();
        
        // Delete Cloudinary images from listing
        await cloudinaryImageService.DeleteImagesAsync(publicIds);
        
        // Delete listing from DB
        var deletedListing = await listingRepo.DeleteListingAsync(listingId); 
        
        if (deletedListing == null)
            throw new InvalidOperationException("Failed to delete listing");
        
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
            ImageUrls = deletedListing.Images.Select(i => i.ImageUrl).ToList(),
            CreatedAt = deletedListing.CreatedAt, 
            UpdatedAt = deletedListing.UpdatedAt, 
        }; 
    }
}