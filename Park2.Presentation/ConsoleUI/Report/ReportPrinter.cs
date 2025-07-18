using Park2.Application.Simulation;
using Park2.Domain.Models;
using System;

namespace Park2.Presentation.ConsoleUI.Report
{
    public interface IReportPrinter
    {
        void PrintTotalRevenue(DateTime simulatedTime);
        void PrintVisitorsForAttraction(DateTime simulatedTime, Attraction attraction);
        void PrintAverageWaitTimeForAttraction(DateTime simulatedTime, Attraction attraction);
        void PrintPeakQueueLengthForAttraction(DateTime simulatedTime, Attraction attraction);
    }

    public class ReportPrinter : IReportPrinter
    {
        private readonly IReportController _reportController;
        private readonly IOutputWriter _writer;

        public ReportPrinter(IReportController reportController, IOutputWriter writer)
        {
            _reportController = reportController ?? throw new ArgumentNullException(nameof(reportController));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void PrintTotalRevenue(DateTime simulatedTime)
        {
            var revenue = _reportController.GetTotalRevenue();
            PrintMetric(simulatedTime, "Общая выручка", $"{revenue} ₸");
        }

        public void PrintVisitorsForAttraction(DateTime simulatedTime, Attraction attraction)
        {
            var count = _reportController.GetTotalVisitorsForAttraction(attraction.Id);
            PrintAttractionMetric(simulatedTime, attraction.Name, "Общее количество посетителей", count.ToString());
        }

        public void PrintAverageWaitTimeForAttraction(DateTime simulatedTime, Attraction attraction)
        {
            var avgWait = _reportController.GetAverageWaitTimeForAttraction(attraction.Id);
            PrintAttractionMetric(simulatedTime, attraction.Name, "Среднее время ожидания (мин.)", avgWait.ToString("0.##"));
        }

        public void PrintPeakQueueLengthForAttraction(DateTime simulatedTime, Attraction attraction)
        {
            var peak = _reportController.GetPeakQueueLengthForAttraction(attraction.Id);
            PrintAttractionMetric(simulatedTime, attraction.Name, "Пиковая длина очереди", peak.ToString());
        }

        private void PrintAttractionMetric(DateTime time, string attractionName, string label, string value)
        {
            _writer.WriteLine($"[{time:HH:mm}] {label} для '{attractionName}': {value}");
            _writer.WriteLine("");
        }

        private void PrintMetric(DateTime time, string label, string value)
        {
            _writer.WriteLine($"[{time:HH:mm}] {label}: {value}");
            _writer.WriteLine("");
        }
    }
}
