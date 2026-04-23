using Application.DTOs.CourseDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/courses")]
public class CourseController(ICourseService courseService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CourseFilterDto filter)
    {
        var result = await courseService.GetAllAsync(filter);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await courseService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [Authorize(Roles = $"{UserRole.Instructor},{UserRole.Admin}")]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddCourseDto dto)
    {
        var result = await courseService.AddAsync(dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = $"{UserRole.Instructor},{UserRole.Admin}")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseDto dto)
    {
        var result = await courseService.UpdateAsync(id, dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = $"{UserRole.Instructor},{UserRole.Admin}")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await courseService.DeleteAsync(id);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [Authorize(Roles = UserRole.Instructor)]
    [HttpPatch("{id}/publish")]
    public async Task<IActionResult> Publish(int id)
    {
        var result = await courseService.PublishAsync(id);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = UserRole.Instructor)]
    [HttpPost("{id}/thumbnail")]
    public async Task<IActionResult> UploadThumbnail(int id, IFormFile file)
    {
        var allowedTypes = new[] { "image/jpeg", "image/png" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest(new { message = "Разрешены только JPEG и PNG" });

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { message = "Файл не должен превышать 5MB" });

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "thumbnails");
        Directory.CreateDirectory(uploadsPath);

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{ext}";
        var filePath = Path.Combine(uploadsPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var imageUrl = $"/uploads/thumbnails/{fileName}";
        return Ok(new { imageUrl });
    }
}