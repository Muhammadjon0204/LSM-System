using Application.DTOs.ReviewDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/courses/{courseId}/reviews")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByCourseId(int courseId)
    {
        var result = await reviewService.GetByCourseIdAsync(courseId);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [Authorize(Roles = UserRole.Student)]
    [HttpPost]
    public async Task<IActionResult> Add(int courseId, [FromBody] AddReviewDto dto)
    {
        dto.CourseId = courseId;
        var result = await reviewService.AddAsync(dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = UserRole.Student)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int courseId, int id, [FromBody] UpdateReviewDto dto)
    {
        var result = await reviewService.UpdateAsync(id, dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = $"{UserRole.Admin},{UserRole.Student}")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int courseId, int id)
    {
        var result = await reviewService.DeleteAsync(id);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }
}