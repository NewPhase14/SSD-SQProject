using Application.Models.Dtos;
using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IListingService
{
    Task<ListingResponseDto> CreateListing(ListingCreateRequestDto dto);
    Task<ListingResponseDto> UpdateListing(ListingUpdateRequestDto dto);
    Task<List<ListingResponseDto>> GetAllListings();
    Task<List<ListingResponseDto>> GetListingsByUserId(string id);
    Task<ListingResponseDto> DeleteListing(string id);
}