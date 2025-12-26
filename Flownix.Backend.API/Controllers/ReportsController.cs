using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Flownix.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IPdfReportService _pdfReportService;

        public ReportsController(IPdfReportService pdfReportService)
        {
            _pdfReportService = pdfReportService;
        }


        [HttpGet("my-object/{waterObjectId:guid}")]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> GetSimpleReport(
     Guid waterObjectId,
     [FromServices] IFlownixDbContext context)
        {
            var userId = GetCurrentUserId();

            // Простая проверка
            if (!User.IsInRole("Admin"))
            {
                var hasAccess = await context.UserObjectAccesses
                    .AnyAsync(ua => ua.UserId == userId && ua.WaterObjectId == waterObjectId);

                if (!hasAccess) return Forbid();
            }

            // Только основные данные
            var waterObject = await context.WaterObjects
                .FirstOrDefaultAsync(wo => wo.Id == waterObjectId);

            if (waterObject == null) return NotFound();

            var reportModel = new WaterObjectReportModel
            {
                WaterObjectId = waterObject.Id,
                WaterObjectName = waterObject.Name,
                ReportDate = DateTime.UtcNow,
                Location = waterObject.Location,
                WaterObjectType = waterObject.Type.ToString(),
                UserEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "operator@example.com",
                UserName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Оператор",
                CurrentStatus = waterObject.IsActive ? "Активний" : "Неактивний",
                CurrentVolume = waterObject.CurrentVolume,
                MaxVolume = waterObject.MaxVolume,
                TemperatureMin = 20,
                TemperatureMax = 25,
                WaterLevelMin = waterObject.CurrentVolume * 0.8,
                WaterLevelMax = waterObject.CurrentVolume * 1.2,
                SummaryConclusion = $"Звіт для {waterObject.Name}. Поточний об'єм: {waterObject.CurrentVolume} м³"
            };

            var pdfBytes = _pdfReportService.GenerateWaterObjectReportPdf(reportModel);
            return File(pdfBytes, "application/pdf", $"report-{waterObject.Name}.pdf");
        }

        #region Helper Methods

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Invalid user ID in token");
        }

        private string GetReadingStatus(double value, double min, double max)
        {
            if (value > max) return "Critical";
            if (value < min) return "Warning";
            return "Normal";
        }

        private string GenerateSummaryConclusion(
            Domain.Entities.WaterObject waterObject,
            List<WaterObjectReportSensorModel> readings)
        {
            var criticalCount = readings.Count(r => r.ReadingStatus == "Critical");
            var warningCount = readings.Count(r => r.ReadingStatus == "Warning");

            if (criticalCount > 0)
                return $"Критичний стан: {criticalCount} показників вище норми. Необхідна негайна увага.";

            if (warningCount > 0)
                return $"Попередження: {warningCount} показників нижче норми. Рекомендується перевірка.";

            return "Об'єкт функціонує в нормальному режимі. Всі показники в межах норми.";
        }

        #endregion
    }
}