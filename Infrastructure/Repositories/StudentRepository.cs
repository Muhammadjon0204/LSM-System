using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StudentRepository(AppDbContext context) : IStudentRepository
{
    public async Task<Student> AddAsync(Student item)
    {
        await context.Students.AddAsync(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<Student> UpdateAsync(Student item)
    {
        context.Students.Update(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteAsync(Student item)
    {
        var exist = await context.Students.FindAsync(item.Id);
        if (exist == null) return false;

        context.Students.Remove(exist);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await context.Students
            .Include(s => s.User)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student?> GetByUserIdAsync(int userId)
    {
        return await context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }
}