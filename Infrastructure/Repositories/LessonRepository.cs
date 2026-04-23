using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LessonRepository(AppDbContext context) : ILessonRepository
{
    public async Task<Lesson> AddAsync(Lesson item)
    {
        await context.Lessons.AddAsync(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<Lesson> UpdateAsync(Lesson item)
    {
        context.Lessons.Update(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteAsync(Lesson item)
    {
        var exist = await context.Lessons.FindAsync(item.Id);
        if (exist == null) return false;

        context.Lessons.Remove(exist);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Lesson>> GetAllAsync()
    {
        return await context.Lessons.AsNoTracking().ToListAsync();
    }

    public async Task<Lesson?> GetByIdAsync(int id)
    {
        return await context.Lessons.FindAsync(id);
    }

    public async Task<List<Lesson>> GetByCourseIdAsync(int courseId)
    {
        return await context.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .AsNoTracking()
            .ToListAsync();
    }
}