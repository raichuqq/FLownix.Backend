using Flownix.Backend.Contracts.DTOs.Reports;

namespace Flownix.Backend.Application.Interfaces
{
    public interface IPdfReportService
    {
        byte[] GenerateWaterObjectReportPdf(WaterObjectReportModel model);
    }
}