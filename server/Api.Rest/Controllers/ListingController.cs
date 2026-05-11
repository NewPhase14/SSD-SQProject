using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class ListingController(IListingService listingService, ISecurityService securityService) : ControllerBase
{
    public const string ControllerRoute = "api/listing/";

    public const string CreateRoute = ControllerRoute + nameof(Create);
    
    public const string UpdateRoute = ControllerRoute + nameof(Update);
    
    public const string DeleteRoute = ControllerRoute + nameof(Delete);
    
    public const string GetAllRoute = ControllerRoute + nameof(GetAll);
    
    public const string GetAllByUserIdRoute = ControllerRoute + nameof(GetAllByUserId);
    
    [HttpPost]
    [Route(CreateRoute)]
    public async Task<ActionResult<ListingResponseDto>> Create([FromBody] ListingCreateRequestDto dto, [FromHeader] string authorization)
    {
        securityService.VerifyJwtOrThrow(authorization);
        return Ok(await listingService.CreateListing(dto));
    }

    [HttpPut]
    [Route(UpdateRoute)]
    public async Task<ActionResult<ListingResponseDto>> Update([FromBody] ListingUpdateRequestDto dto, [FromHeader] string authorization)
    {
        var jwt = securityService.VerifyJwtOrThrow(authorization);
        if (jwt.Id != dto.UserId)
        {
            return Unauthorized();
        }
        return Ok(await listingService.UpdateListing(dto));
    }

    [HttpDelete]
    [Route(DeleteRoute)]
    public async Task<ActionResult<ListingResponseDto>> Delete(string id, [FromHeader] string authorization)
    {
        securityService.VerifyJwtOrThrow(authorization);
        return Ok(await listingService.DeleteListing(id));
    }

    [HttpGet]
    [Route(GetAllRoute)]
    public async Task<ActionResult<List<ListingResponseDto>>> GetAll()
    {
        return Ok(await listingService.GetAllListings());
    }

    [HttpGet]
    [Route(GetAllByUserIdRoute)]
    public async Task<ActionResult<List<ListingResponseDto>>> GetAllByUserId(string id, [FromHeader] string authorization)
    {
        securityService.VerifyJwtOrThrow(authorization);
        return Ok(await listingService.GetListingsByUserId(id));
    }
    
}