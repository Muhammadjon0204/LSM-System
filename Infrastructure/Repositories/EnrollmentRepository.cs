using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EnrollmentRepository(AppDbContext context) : IEnrollmentRepository
{
    public async Task<Enrollment> AddAsync(Enrollment item)
    {
        await context.Enrollments.AddAsync(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<Enrollment> UpdateAsync(Enrollment item)
    {
        context.Enrollments.Update(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteAsync(Enrollment item)
    {
        var exist = await context.Enrollments.FindAsync(item.Id);
        if (exist == null) return false;

        context.Enrollments.Remove(exist);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Enrollment>> GetAllAsync()
    {
        return await context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.Student)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Enrollment?> GetByIdAsync(int id)
    {
        return await context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.Student)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Enrollment?> GetByStudentAndCourseAsync(int studentId, int courseId)
    {
        return await context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
    }

    public async Task<List<Enrollment>> GetByStudentIdAsync(int studentId)
    {
        return await context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId)
            .AsNoTracking()
            .ToListAsync();
    }
}