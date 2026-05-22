using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Category;

namespace Application.Services;

public class CategoryService(ICategoryRepo categoryRepo) : ICategoryService
{
    public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var categories = await categoryRepo.GetAllCategoriesAsync();

        if (categories is null)
            throw new InvalidOperationException("Categories not fund");
        
        return categories.Select(c => new CategoryResponseDto
        { 
            Id = c.Id,
            Name = c.Name
        }).ToList();
    }
}