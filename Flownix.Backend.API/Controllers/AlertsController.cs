// Flownix.Backend.API/Controllers/AlertsController.cs
using Flownix.Backend.API.Common;
using Flownix.Backend.Application.Services.Alerts.Queries;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flownix.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlertsController : BaseController
    {
        // GET: api/alerts
        [HttpGet]
        public async Task<ActionResult<List<AlertDto>>> GetAllAlerts()
        {
            var query = new GetAllAlertsQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AlertDto>> GetAlertById(Guid id)
        {
            var query = new GetAlertByIdQuery(id);
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}