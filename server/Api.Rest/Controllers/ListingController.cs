using Application.Interfaces;
using Application.Models.Dtos.Listings;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class ListingController(IListingService listingService, IJwtService jwtService) : ControllerBase
{
    private const string ControllerRoute = "api/listing/";

    private const string CreateRoute = ControllerRoute + nameof(Create);
    
    private const string UpdateRoute = ControllerRoute + nameof(Update);
    
    private const string DeleteRoute = ControllerRoute + nameof(Delete);
    
    private const string GetAllRoute = ControllerRoute + nameof(GetAll);
    
    private const string GetAllByUserIdRoute = ControllerRoute + nameof(GetAllByUserId);
    
    [HttpPost]
    [Route(CreateRoute)]
    public async Task<ActionResult<ListingResponseDto>> Create([FromForm] ListingCreateRequestDto dto, [FromHeader] string authorization)
    {
        var jwt = jwtService.VerifyJwtOrThrow(authorization);
        if (jwt.Type == "Auth")
        {
            return Ok(await listingService.CreateListingAsync(dto, jwt.Id));
        }
        return Unauthorized();
    }

    [HttpPut]
    [Route(UpdateRoute)]
    public async Task<ActionResult<ListingResponseDto>> Update([FromBody] ListingUpdateRequestDto dto, [FromHeader] string authorization)
    {
        var jwt = jwtService.VerifyJwtOrThrow(authorization);
        if (jwt.Type == "Auth")
        {
            return Ok(await listingService.UpdateListingAsync(dto, jwt.Id));
        }
        return Unauthorized();
    }

    [HttpDelete]
    [Route(DeleteRoute)]
    public async Task<ActionResult<ListingResponseDto>> Delete(string listingId, [FromHeader] string authorization)
    {
        var jwt = jwtService.VerifyJwtOrThrow(authorization);
        if (jwt.Type == "Auth")
        {
            return Ok(await listingService.DeleteListingAsync(listingId, jwt.Id));
        }
        return Unauthorized();
    }

    [HttpGet]
    [Route(GetAllRoute)]
    public async Task<ActionResult<List<ListingResponseDto>>> GetAll()
    {
        return Ok(await listingService.GetAllListingsAsync());
    }

    [HttpGet]
    [Route(GetAllByUserIdRoute)]
    public async Task<ActionResult<List<ListingResponseDto>>> GetAllByUserId([FromHeader] string authorization)
    {
        var jwt = jwtService.VerifyJwtOrThrow(authorization);
        if (jwt.Type == "Auth")
        {
            return Ok(await listingService.GetListingsByUserIdAsync(jwt.Id));
        }
        return Unauthorized();
    }
    
}