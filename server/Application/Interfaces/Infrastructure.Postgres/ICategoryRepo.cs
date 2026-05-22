using Application.Models.Dtos.Category;
using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface ICategoryRepo
{
    Task<List<Category>> GetAllCategoriesAsync();
}