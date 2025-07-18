using Moq;
using Park2.Application.Interfaces.AttractionInterface;
using Park2.Application.Services.ReportServices;
using Park2.Domain.Models;
using Park2.Domain.Enums;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Xunit;
using Castle.Core.Logging;

namespace Park2.Tests.Services
{
    public class ReportServiceTests
    {
        private readonly Mock<IAttractionService> _mockAttractionService;
        private readonly Park _park;
        private readonly ReportService _reportService;
        private readonly ILogger<ReportService> _logger;

        public ReportServiceTests()
        {
            _mockAttractionService = new Mock<IAttractionService>();
            _logger = new LoggerFactory().CreateLogger<ReportService>();

            _park = new Park
            {
                Attractions = new List<Attraction>(),
                TotalRevenue = 0
            };

            _reportService = new ReportService(_mockAttractionService.Object, _park, _logger);
        }

        private Attraction CreateAttraction(Guid id, decimal ticketPrice = 1000, int totalVisitors = 0, int peakQueueLength = 5, List<TimeSpan>? waitTimes = null)
        {
            return new Attraction
            {
                Id = id,
                Name = "Test",
                Capacity = 10,
                Duration = TimeSpan.FromMinutes(10),
                MinAge = 0,
                TicketPrice = ticketPrice,
                TotalVisitors = totalVisitors,
                PeakQueueLength = peakQueueLength,
                WaitTimes = waitTimes ?? new List<TimeSpan>(),
                Status = AttractionStatus.Open
            };
        }

        [Fact]
        public void GetTotalRevenue_ReturnsCorrectAmount()
        {
            // Arrange
            _park.TotalRevenue = 5000;

            // Act
            var result = _reportService.GetTotalRevenue();

            // Assert
            Assert.Equal(5000, result);
        }

        [Fact]
        public void IncreaseTotalRevenue_AddsAmount()
        {
            // Arrange
            _reportService.IncreaseTotalRevenue(2000);

            // Act
            var result = _reportService.GetTotalRevenue();

            // Assert
            Assert.Equal(2000, result);
        }

        [Fact]
        public void GetMoneyFromAttractionLaunch_ReturnsCorrectSum()
        {
            // Arrange
            var id = Guid.NewGuid();
            var attraction = CreateAttraction(id, ticketPrice: 1500);
            _park.Attractions.Add(attraction);

            // Act
            var result = _reportService.GetMoneyFromAttractionLaunch(id, 3);

            // Assert
            Assert.Equal(4500, result);
        }

        [Fact]
        public void GetTotalVisitorsForAttraction_ReturnsCorrectAmount()
        {
            // Arrange
            var id = Guid.NewGuid();
            var attraction = CreateAttraction(id, totalVisitors: 12);
            _park.Attractions.Add(attraction);

            // Act
            var result = _reportService.GetTotalVisitorsForAttraction(id);

            // Assert
            Assert.Equal(12, result);
        }

        [Fact]
        public void IncreaseTotalVisitorsForAttraction_AddsCorrectAmount()
        {
            // Arrange
            var id = Guid.NewGuid();
            var attraction = CreateAttraction(id, totalVisitors: 5);
            _park.Attractions.Add(attraction);

            // Act
            _reportService.IncreaseTotalVisitorsForAttraction(id, 3);

            // Assert
            Assert.Equal(8, attraction.TotalVisitors);
        }

        [Fact]
        public void GetAverageWaitTimeForAttraction_CalculatesCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var waitTimes = new List<TimeSpan> { TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(10) };
            var attraction = CreateAttraction(id, waitTimes: waitTimes);
            _park.Attractions.Add(attraction);

            // Act
            var result = _reportService.GetAverageWaitTimeForAttraction(id);

            // Assert
            Assert.Equal(10.00m, result);
        }

        [Fact]
        public void GetAverageWaitTimeForAttraction_ReturnsZero_WhenNoWaitTimes()
        {
            // Arrange
            var id = Guid.NewGuid();
            var attraction = CreateAttraction(id);
            _park.Attractions.Add(attraction);

            // Act
            var result = _reportService.GetAverageWaitTimeForAttraction(id);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetPeakQueueLengthForAttraction_ReturnsCorrectValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var attraction = CreateAttraction(id, peakQueueLength: 12);
            _park.Attractions.Add(attraction);

            // Act
            var result = _reportService.GetPeakQueueLengthForAttraction(id);

            // Assert
            Assert.Equal(12, result);
        }

        [Fact]
        public void GetPeakQueueLengthForAttraction_ReturnsZero_WhenAttractionNotFound()
        {
            // Act
            var result = _reportService.GetPeakQueueLengthForAttraction(Guid.NewGuid());

            // Assert
            Assert.Equal(0, result);
        }
    }
}
