using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Extentsions;

public static class ResultExtensions
{
    
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        return result.StatusCode switch
        {
            200 => controller.Ok(result),
            201 => controller.StatusCode(201, result),
            400 => controller.BadRequest(result),
            401 => controller.Unauthorized(result),
            403 => controller.StatusCode(403, result),
            404 => controller.NotFound(result),
            _   => controller.StatusCode(result.StatusCode, result)
        };
    }
    
}