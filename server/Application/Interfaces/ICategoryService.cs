using Application.Models.Dtos.Category;

namespace Application.Interfaces;

public interface ICategoryService
{
    public Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
}