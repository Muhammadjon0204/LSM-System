using Application.Interfaces.BaseInterfaces;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ILessonRepository : IBaseRepository<Lesson>
{
    Task<List<Lesson>> GetByCourseIdAsync(int courseId);
}