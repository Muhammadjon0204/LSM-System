using Application.DTOs.LessonDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/courses/{courseId}/lessons")]
public class LessonController(ILessonService lessonService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetByCourseId(int courseId)
    {
        var result = await lessonService.GetByCourseIdAsync(courseId);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int courseId, int id)
    {
        var result = await lessonService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [Authorize(Roles = UserRole.Instructor)]
    [HttpPost]
    public async Task<IActionResult> Add(int courseId, [FromBody] AddLessonDto dto)
    {
        var result = await lessonService.AddAsync(courseId, dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = UserRole.Instructor)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int courseId, int id, [FromBody] UpdateLessonDto dto)
    {
        var result = await lessonService.UpdateAsync(id, dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = $"{UserRole.Instructor},{UserRole.Admin}")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int courseId, int id)
    {
        var result = await lessonService.DeleteAsync(id);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }
}