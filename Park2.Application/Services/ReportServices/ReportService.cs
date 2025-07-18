using Microsoft.Extensions.Logging;
using Park2.Application.Interfaces.AttractionInterface;
using Park2.Application.Interfaces.ReportInterfaces;
using Park2.Application.Simulation;
using Park2.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Application.Services.ReportServices
{
    public class ReportService : IReportService
    {
        private readonly IAttractionService _attractionService;
        private readonly Park _park;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IAttractionService attractionService, Park park, ILogger<ReportService> logger)
        {
            _attractionService = attractionService;
            _park = park;
            _logger = logger;
        }

        public decimal GetTotalRevenue()
        {
            try
            {
                return _park?.TotalRevenue ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении общего дохода");
                return 0;
            }
        }

        public void IncreaseTotalRevenue(decimal sum)
        {
            try
            {
                if (_park != null)
                {
                    _park.TotalRevenue += sum;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при увеличении общего дохода");
            }
        }

        public decimal GetMoneyFromAttractionLaunch(Guid attractionId, int visitorsAmount)
        {
            var attraction = TryGetAttraction(attractionId, logIfNotFound: true);

            return attraction.TicketPrice * visitorsAmount;
        }

        public int GetTotalVisitorsForAttraction(Guid attractionId)
        {
            var attraction = TryGetAttraction(attractionId, logIfNotFound: true);

            var totalVisitors = attraction.TotalVisitors;

            return totalVisitors;
        }

        public void IncreaseTotalVisitorsForAttraction(Guid attractionId, int visitorsAmount)
        {
            var attraction = TryGetAttraction(attractionId, logIfNotFound: true); 

            attraction.TotalVisitors += visitorsAmount;
        }

        private Attraction? TryGetAttraction(Guid attractionId, bool logIfNotFound = false)
        {
            try
            {
                var attraction = _park.Attractions.FirstOrDefault(a => a.Id == attractionId);

                if (attraction == null && logIfNotFound)
                {
                    _logger.LogWarning("Аттракцион с Id {AttractionId} не найден", attractionId);
                }

                return attraction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении аттракциона с Id {AttractionId}", attractionId);
                return null;
            }
        }

        public decimal GetAverageWaitTimeForAttraction(Guid attractionId)
        {
            var attraction = TryGetAttraction(attractionId, logIfNotFound: true);

            if (attraction == null || attraction.WaitTimes.Count == 0)
                return 0;

            return Math.Round((decimal)attraction.WaitTimes.Average(w => w.TotalMinutes), 2);
        }

        public int GetPeakQueueLengthForAttraction(Guid attractionId)
        {
            var attraction = TryGetAttraction(attractionId, logIfNotFound: true);

            if (attraction == null)
            {
                return 0;
            }

            return attraction.PeakQueueLength;
        }
    }
}
