using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Postgresql.Data;

public class UserRepo(MyDbContext ctx) : IUserRepo
{
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await ctx.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        return await ctx.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> AddUserAsync(User user)
    {
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();
        return user;
    }
    

    public async Task<User> UpdateUserAsync(User user)
    {
        ctx.Users.Update(user);
        await ctx.SaveChangesAsync();
        return user;
    }

    public async Task<User?> DeleteUserAsync(string userId)
    {
        var user = ctx.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null) return null;
        
        ctx.Users.Remove(user);
        await ctx.SaveChangesAsync();
        return user;
    }
}