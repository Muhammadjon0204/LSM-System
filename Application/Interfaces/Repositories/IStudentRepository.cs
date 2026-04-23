using Application.Interfaces.BaseInterfaces;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IStudentRepository : IBaseRepository<Student>
{
    Task<Student?> GetByUserIdAsync(int userId);
}