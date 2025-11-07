using Microsoft.AspNetCore.Mvc;
using TaskManagment.Enum;
using TaskManagment.Services;

namespace TaskManagment.Helpers
{
    public static class ControllerResponseHelper
    {
        public static ActionResult ResolveResult<T>(ControllerBase controller, ServiceResult<T> result)
        {
            return result.Status switch
            {
                ServiceStatus.Success => controller.Ok(result.Data),
                ServiceStatus.NotFound => controller.NotFound(result.Message ?? "Resource not found."),
                ServiceStatus.ValidationError => controller.BadRequest(result.Message ?? "Invalid request."),
                ServiceStatus.Conflict => controller.Conflict(result.Message ?? "Conflict detected."),
                _ => controller.StatusCode(500, result.Message ?? "An unexpected error occurred.")
            };
        }
    }
}

