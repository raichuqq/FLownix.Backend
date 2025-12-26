using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Flownix.Backend.Infrastructure.Integration.Reporting
{
    public class PdfReportService : IPdfReportService
    {
        public PdfReportService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateWaterObjectReportPdf(WaterObjectReportModel model)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Content().Column(column =>
                    {
                        column.Item().Text("Звіт Flownix").FontSize(20).Bold();
                        column.Item().Text($"Об'єкт: {model.WaterObjectName}");
                        column.Item().Text($"Тип: {model.WaterObjectType}");
                        column.Item().Text($"Локація: {model.Location}");
                        column.Item().Text($"Користувач: {model.UserName}");
                        column.Item().Text($"Рівень води: {model.CurrentVolume} / {model.MaxVolume} м³");
                        column.Item().Text($"Висновок: {model.SummaryConclusion}");
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}