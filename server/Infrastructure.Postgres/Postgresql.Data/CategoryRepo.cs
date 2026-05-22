using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Category;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Postgresql.Data;

public class CategoryRepo(MyDbContext ctx): ICategoryRepo
{
    public async Task<List<Category>> GetAllCategoriesAsync()
    {
       return await ctx.Categories.ToListAsync();
    }
}