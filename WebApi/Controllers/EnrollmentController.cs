using Application.DTOs.EnrollmentDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[ApiController]
[Route("api/enrollments")]
[Authorize]
public class EnrollmentController(IEnrollmentService enrollmentService) : ControllerBase
{
    [Authorize(Roles = UserRole.Student)]
    [HttpPost]
    public async Task<IActionResult> Enroll([FromBody] AddEnrollmentDto dto)
    {
        var result = await enrollmentService.EnrollAsync(dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = UserRole.Student)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await enrollmentService.CancelAsync(id);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [Authorize(Roles = UserRole.Student)]
    [HttpPatch("{id}/progress")]
    public async Task<IActionResult> UpdateProgress(int id, [FromBody] UpdateProgressDto dto)
    {
        var result = await enrollmentService.UpdateProgressAsync(id, dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = UserRole.Student)]
    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await enrollmentService.GetByStudentIdAsync(studentId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}