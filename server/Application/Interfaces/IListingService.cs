using Application.Models.Dtos;
using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IListingService
{
    Task<ListingResponseDto> CreateListing(ListingCreateRequestDto dto);
    Task<ListingResponseDto> UpdateListing(ListingUpdateRequestDto dto);
    Task<List<Listing>> GetAllListings();
    Task<ListingResponseDto> GetListingByUserId(string id);
    Task<ListingResponseDto> DeleteListing(string id);
}