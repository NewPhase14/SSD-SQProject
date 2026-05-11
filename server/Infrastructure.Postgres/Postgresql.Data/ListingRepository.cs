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

    public Task<List<Listing>> GetAllListings()
    {
        throw new NotImplementedException();
    }

    public Task<Listing> GetListingByUserId(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Listing> DeleteListing(string id)
    {
        throw new NotImplementedException();
    }
}