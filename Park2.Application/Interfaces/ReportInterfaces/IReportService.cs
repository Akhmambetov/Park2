using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Application.Interfaces.ReportInterfaces
{
    public interface IReportService
    {
        decimal GetTotalRevenue();
        void IncreaseTotalRevenue(decimal sum);
        decimal GetMoneyFromAttractionLaunch(Guid attractionId, int visitorsAmount);
        int GetTotalVisitorsForAttraction(Guid attractionId);
        void IncreaseTotalVisitorsForAttraction(Guid attractionId, int visitorsAmount);
        decimal GetAverageWaitTimeForAttraction(Guid attractionId);
        int GetPeakQueueLengthForAttraction(Guid attractionId);
    }
}
