using Application.DTOs.StudentDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]
public class StudentController(IStudentService studentService) : ControllerBase
{
    [Authorize(Roles = UserRole.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await studentService.GetAllAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = $"{UserRole.Admin},{UserRole.Student}")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await studentService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddStudentDto dto)
    {
        var result = await studentService.AddAsync(dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = $"{UserRole.Admin},{UserRole.Student}")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto dto)
    {
        var result = await studentService.UpdateAsync(id, dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await studentService.DeleteAsync(id);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }
}