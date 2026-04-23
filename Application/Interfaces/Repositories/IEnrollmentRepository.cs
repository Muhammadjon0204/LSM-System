using Application.Interfaces.BaseInterfaces;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IEnrollmentRepository : IBaseRepository<Enrollment>
{
    Task<Enrollment?> GetByStudentAndCourseAsync(int studentId, int courseId);
    Task<List<Enrollment>> GetByStudentIdAsync(int studentId);   
}