using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos;
using Application.Models.Dtos.Listings;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class ListingController(IListingService listingService, ISecurityService securityService) : ControllerBase
{
    private const string ControllerRoute = "api/listing/";

    private const string CreateRoute = ControllerRoute + nameof(Create);
    
    private const string UpdateRoute = ControllerRoute + nameof(Update);
    
    private const string DeleteRoute = ControllerRoute + nameof(Delete);
    
    private const string GetAllRoute = ControllerRoute + nameof(GetAll);
    
    private const string GetAllByUserIdRoute = ControllerRoute + nameof(GetAllByUserId);
    
    [HttpPost]
    [Route(CreateRoute)]
    public async Task<ActionResult<ListingResponseDto>> Create([FromBody] ListingCreateRequestDto dto, [FromHeader] string authorization)
    {
        var jwt = securityService.VerifyJwtOrThrow(authorization);
        return Ok(await listingService.CreateListing(dto, jwt.Id));
    }

    [HttpPut]
    [Route(UpdateRoute)]
    public async Task<ActionResult<ListingResponseDto>> Update([FromBody] ListingUpdateRequestDto dto, [FromHeader] string authorization)
    {
        var jwt = securityService.VerifyJwtOrThrow(authorization);
        return Ok(await listingService.UpdateListing(dto, jwt.Id));
    }

    [HttpDelete]
    [Route(DeleteRoute)]
    public async Task<ActionResult<ListingResponseDto>> Delete(string listingId, [FromHeader] string authorization)
    {
        var jwt = securityService.VerifyJwtOrThrow(authorization);
        return Ok(await listingService.DeleteListing(listingId, jwt.Id));
    }

    [HttpGet]
    [Route(GetAllRoute)]
    public async Task<ActionResult<List<ListingResponseDto>>> GetAll()
    {
        return Ok(await listingService.GetAllListings());
    }

    [HttpGet]
    [Route(GetAllByUserIdRoute)]
    public async Task<ActionResult<List<ListingResponseDto>>> GetAllByUserId([FromHeader] string authorization)
    {
        var jwt = securityService.VerifyJwtOrThrow(authorization);
        return Ok(await listingService.GetListingsByUserId(jwt.Id));
    }
    
}