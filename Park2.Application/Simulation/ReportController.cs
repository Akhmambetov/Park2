using Park2.Application.Interfaces.ReportInterfaces;
using Park2.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Application.Simulation
{
    public class ReportController
    {
        private readonly Park _park;
        private readonly IReportService _reportService;

        public ReportController(Park park, IReportService reportService)
        {
            _park = park;
            _reportService = reportService;
        }

        public decimal GetTotalRevenue()
        {
            decimal totalRevenue = _reportService.GetTotalRevenue();

            return totalRevenue;
        }

        public int GetTotalVisitorsForAttraction(Guid attractionId)
        {
            int totalVisitorsForAttraction = _reportService.GetTotalVisitorsForAttraction(attractionId);

            return totalVisitorsForAttraction;
        }

        public decimal GetAverageWaitTimeForAttraction(Guid attractionId)
        {
            decimal averageWaitTime = _reportService.GetAverageWaitTimeForAttraction(attractionId);

            return averageWaitTime;
        }

        public int GetPeakQueueLengthForAttraction(Guid attractionId)
        {
            int peakLength = _reportService.GetPeakQueueLengthForAttraction(attractionId);

            return peakLength;
        }
    }
}
