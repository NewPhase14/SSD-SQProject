using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Postgresql.Data;


public class ListingRepository(MyDbContext ctx) : IListingRepository
{
    public async Task<Listing> CreateListing(Listing listing)
    {
        await ctx.Listings.AddAsync(listing);
        await ctx.SaveChangesAsync();
        return listing;
    }

    public async Task<Listing> UpdateListing(Listing listing)
    {
        var existingListing = await ctx.Listings.FirstOrDefaultAsync(l => l.Id == listing.Id);
        if (existingListing == null)
            throw new InvalidOperationException("Listing not found");

        existingListing.CategoryId = listing.CategoryId;
        existingListing.Condition = listing.Condition;
        existingListing.Title = listing.Title;
        existingListing.Description = listing.Description;
        existingListing.Price = listing.Price;
        existingListing.Status = listing.Status;
        existingListing.UpdatedAt = DateTime.UtcNow;

        var updatedListing = ctx.Listings.Update(existingListing);
        await ctx.SaveChangesAsync();
        return updatedListing.Entity;
    }

    public async Task<List<Listing>> GetAllListings()
    {
        var listings = await ctx.Listings.ToListAsync();
        return listings;
    }

    public async Task<List<Listing>> GetListingByUserId(string id)
    {
        var listing = await ctx.Listings.Where(l => l.UserId == id).ToListAsync();
        if (listing.Count == 0)
            throw new InvalidOperationException("No listings found");
        return listing;
    }

    public async Task<Listing> DeleteListing(string id)
    {
        var listing = await ctx.Listings.FirstOrDefaultAsync(l => l.Id == id);
        if (listing == null)
            throw new InvalidOperationException("Listing not found");
        ctx.Remove(listing);
        await ctx.SaveChangesAsync();
        return listing;
    }

    public async Task<List<Image>> AddImages(List<Image> images)
    {
        await ctx.Images.AddRangeAsync(images);
        await ctx.SaveChangesAsync();
        return images;
    }

    public async Task<string?> GetSellerIdAsync(string listingId)
    {
        var sellerId = await ctx.Listings.Where(l => l.Id == listingId).Select(l => l.UserId).FirstOrDefaultAsync();
        return sellerId;
    }
}