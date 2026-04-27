using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<Category> AddAsync(Category item)
    {
        await context.Categories.AddAsync(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<Category> UpdateAsync(Category item)
    {
        context.Categories.Update(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteAsync(Category item)
    {
        var exist = await context.Categories.FindAsync(item.Id);
        if (exist == null)
        {
            return false;
        }

        context.Categories.Remove(exist);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await context.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        var existCategory = await context.Categories.FindAsync(id);
        if (existCategory == null)
        {
            return null;
        }

        return existCategory;
    }
}