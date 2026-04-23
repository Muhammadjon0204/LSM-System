using Application.Interfaces.BaseInterfaces;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IReviewRepository:IBaseRepository<Review>
{
    Task<List<Review>> GetByCourseIdAsync(int courseId);
    Task<Review?> GetByStudentAndCourseAsync(int studentId, int courseId);
}