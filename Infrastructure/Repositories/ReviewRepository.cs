using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReviewRepository(AppDbContext context) : IReviewRepository
{
    public async Task<Review> AddAsync(Review item)
    {
        await context.Reviews.AddAsync(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<Review> UpdateAsync(Review item)
    {
        context.Reviews.Update(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteAsync(Review item)
    {
        var exist = await context.Reviews.FindAsync(item.Id);
        if (exist == null) return false;

        context.Reviews.Remove(exist);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Review>> GetAllAsync()
    {
        return await context.Reviews.AsNoTracking().ToListAsync();
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        return await context.Reviews.FindAsync(id);
    }

    public async Task<List<Review>> GetByCourseIdAsync(int courseId)
    {
        return await context.Reviews
            .Include(r => r.Student)
            .Where(r => r.CourseId == courseId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Review?> GetByStudentAndCourseAsync(int studentId, int courseId)
    {
        return await context.Reviews
            .FirstOrDefaultAsync(r => r.StudentId == studentId && r.CourseId == courseId);
    }
}