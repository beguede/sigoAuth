using Flunt.Notifications;
using Microsoft.AspNetCore.Mvc;
using AuthService.Application.Models;
using System.Collections.Generic;

namespace AuthService.Api.Controllers
{
    public abstract class ApiBaseController : ControllerBase
    {
        protected BadRequestObjectResult BadRequest(IReadOnlyCollection<Notification> notifications)
        {
            return new BadRequestObjectResult(new ErrorModel(notifications));
        }

        protected NotFoundObjectResult NotFound(string message)
        {
            return new NotFoundObjectResult(new ErrorModel(message));
        }
        protected IActionResult Forbidden()
        {
            return StatusCode(403, "Sem permissão de acesso.");
        }

        protected IActionResult InternalServerError()
        {
            return StatusCode(500, "Ocorreu um erro interno. Não foi possivel comunicar com o servidor.");
        }
    }
}