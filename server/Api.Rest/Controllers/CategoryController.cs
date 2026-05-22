using Application.Interfaces;
using Application.Models.Dtos.Category;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    private const string ControllerRoute = "/api/category";

    private const string GetAllCategories = ControllerRoute + nameof(GetCategories);

    [HttpGet]
    [Route(GetAllCategories)]
    public async Task<ActionResult<List<CategoryResponseDto>>> GetCategories()
    {
        return Ok(await categoryService.GetAllCategoriesAsync());
    }

}