using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Postgresql.Data;


public class ListingRepo(MyDbContext ctx) : IListingRepo
{
    public async Task<Listing> CreateListingAsync(Listing listing)
    {
        await ctx.Listings.AddAsync(listing);
        await ctx.SaveChangesAsync();
        return listing;
    }
    
    public async Task<Listing?> UpdateListingAsync(Listing listing)
    {
        var existingListing = await ctx.Listings.Include(l => l.Images).FirstOrDefaultAsync(l => l.Id == listing.Id);
        if (existingListing == null)
            return null;
        
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

    public async Task<List<Listing>> GetAllListingsAsync()
    {
        var listings = await ctx.Listings.Include(l => l.Images).ToListAsync();
        return listings;
    }

    public async Task<List<Listing>> GetListingByUserIdAsync(string id)
    {
        var listing = await ctx.Listings.Include(l => l.Images).Where(l => l.UserId == id).ToListAsync();
        return listing;
    }

    public async Task<Listing?> DeleteListingAsync(string id)
    {
        var listing = await ctx.Listings.Include(l => l.Images).FirstOrDefaultAsync(l => l.Id == id);
        if (listing == null)
            return null;

        ctx.Remove(listing);
        await ctx.SaveChangesAsync();
        return listing;
    }

    public async Task<string?> GetSellerIdAsync(string listingId)
    {
        var sellerId = await ctx.Listings.Where(l => l.Id == listingId).Select(l => l.UserId).FirstOrDefaultAsync();
        return sellerId;
    }

    public async Task<Listing?> GetListingByIdAsync(string listingId)
    {
        var listing = await ctx.Listings.Include(l => l.Images).FirstOrDefaultAsync(l => l.Id == listingId);
        return listing;
    }
}