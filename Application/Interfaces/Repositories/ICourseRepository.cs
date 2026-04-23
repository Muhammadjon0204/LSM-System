using Application.Common;
using Application.DTOs.CourseDTOs;
using Application.Interfaces.BaseInterfaces;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICourseRepository : IBaseRepository<Course>
{
    Task<PagedResult<Course>> GetFilteredAsync(CourseFilterDto filter);
    Task<Course?> GetWithDetailsAsync(int id);     
}