using Application.Common;
using Application.DTOs.CourseDTOs;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CourseRepository(AppDbContext context) : ICourseRepository
{
    public async Task<Course> AddAsync(Course item)
    {
        await context.Courses.AddAsync(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<Course> UpdateAsync(Course item)
    {
        context.Courses.Update(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteAsync(Course item)
    {
        var exist = await context.Courses.FindAsync(item.Id);
        if (exist == null) return false;

        context.Courses.Remove(exist);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Course>> GetAllAsync()
    {
        return await context.Courses
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        return await context.Courses.FindAsync(id);
    }

    public async Task<Course?> GetWithDetailsAsync(int id)
    {
        return await context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<PagedResult<Course>> GetFilteredAsync(CourseFilterDto filter)
    {
        var query = context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(c => c.Title.Contains(filter.Search) ||
                                     (c.Description != null && c.Description.Contains(filter.Search)));

        if (filter.CategoryId.HasValue)
            query = query.Where(c => c.CategoryId == filter.CategoryId);

        if (filter.Level.HasValue)
            query = query.Where(c => c.Level == filter.Level);

        if (filter.MinPrice.HasValue)
            query = query.Where(c => c.Price >= filter.MinPrice);

        if (filter.MaxPrice.HasValue)
            query = query.Where(c => c.Price <= filter.MaxPrice);

        if (filter.IsPublished.HasValue)
            query = query.Where(c => c.IsPublished == filter.IsPublished);

        query = filter.SortBy switch
        {
            "price" => filter.SortDescending
                ? query.OrderByDescending(c => c.Price)
                : query.OrderBy(c => c.Price),
            "createdAt" => filter.SortDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return PagedResult<Course>.Create(items, total, filter.Page, filter.PageSize);
    }
}