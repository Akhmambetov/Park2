using Moq;
using Xunit;
using Park2.Application.Simulation;
using Park2.Application.Interfaces;
using Park2.Application.Interfaces.VisitorInterface;
using Park2.Application.Interfaces.AttractionInterface;
using Park2.Application.Services.AttrationService;
using Park2.Domain.Models;
using Park2.Domain.Enums;
using Park2.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Park2.Application.Interfaces.ReportInterfaces;

namespace Park2.Tests.Simulation
{
    public class SimulationEngineTests
    {
        private readonly Mock<IClockSimulationService> _mockClock = new();
        private readonly Mock<IVisitorService> _mockVisitorService = new();
        private readonly Mock<IAttractionService> _mockAttractionService = new();
        private readonly Mock<ILogger<SimulationEngine>> _mockLogger = new();
        private readonly Park _park;
        private readonly SimulationSettings _settings;
        private readonly SimulationEngine _engine;

        public SimulationEngineTests()
        {
            _park = new Park { Attractions = new List<Attraction>() };
            _settings = new SimulationSettings { MaxVisitors = 1000, MinuteDurationMs = 1000 };

            var mockOptions = new Mock<IOptions<SimulationSettings>>();
            mockOptions.Setup(o => o.Value).Returns(_settings);

            var mockReportService = new Mock<IReportService>();
            var mockProcessorLogger = new Mock<ILogger<AttractionProcessorService>>();

            var processor = new AttractionProcessorService(
                _park,
                _mockClock.Object,
                _mockVisitorService.Object,
                mockProcessorLogger.Object,
                mockReportService.Object
            );

            _engine = new SimulationEngine(
                _mockClock.Object,
                _park,
                _mockVisitorService.Object,
                processor,
                _mockLogger.Object,
                mockOptions.Object,
                _mockAttractionService.Object
            );
        }

        [Fact]
        public void StartVisitorGeneration_EnablesVisitorFlag()
        {
            _engine.StartVisitorGeneration();
        }

        [Fact]
        public void StopVisitorGeneration_DisablesVisitorFlag()
        {
            _engine.StopVisitorGeneration();
        }

        [Fact]
        public void AddManualVisitor_AddsVisitorToPark()
        {
            // Arrange
            var attraction = new Attraction { Id = Guid.NewGuid(), Name = "Test", Capacity = 5, Duration = TimeSpan.FromMinutes(2), TicketPrice = 300, MinAge = 3, Status = AttractionStatus.Open };
            var visitor = new Visitor { Name = "John", Age = 25, ArrivalTime = DateTime.Now, IsVIP = true};

            _mockClock.Setup(c => c.CurrentTime).Returns(DateTime.Today);
            _mockVisitorService.Setup(v => v.CreateVisitorFromConsole(It.IsAny<DateTime>(), "John", 25, false, attraction)).Returns(visitor);
            _mockVisitorService.Setup(v => v.RouteVisitor(visitor, It.IsAny<IEnumerable<Attraction>>()))
                   .Returns(VisitorRoutingResult.Success); 


            // Act
            _engine.AddManualVisitor("John", 25, false, attraction);

            // Assert
            Assert.Contains(visitor, _park.ActiveVisitors);
        }

        [Fact]
        public void TryChangeAttractionStatus_ToClosed_CallsHandleClosure()
        {
            // Arrange
            var attraction = new Attraction { Id = Guid.NewGuid(), Name = "Wheel", Capacity = 3, Duration = TimeSpan.FromMinutes(3), TicketPrice = 500, MinAge = 3, Status = AttractionStatus.Open };
            _mockAttractionService.Setup(a => a.SetAttractionStatus(attraction, AttractionStatus.Closed)).Returns(true);

            // Act
            var result = _engine.TryChangeAttractionStatus(attraction, AttractionStatus.Closed);

            // Assert
            Assert.True(result);
            _mockAttractionService.Verify(a => a.ClearVisitorsFromAttraction(attraction), Times.Once);
        }

        [Fact]
        public void TryChangeAttractionStatus_ToMaintenance_SetsToOpenLater()
        {
            // Arrange
            var attraction = new Attraction {Id = Guid.NewGuid(), Name = "Drop Tower", Capacity = 6, Duration = TimeSpan.FromMinutes(1), TicketPrice = 700, MinAge = 3, Status = AttractionStatus.Open };
            _mockAttractionService.Setup(a => a.SetAttractionStatus(attraction, AttractionStatus.Maintenance)).Returns(true);
            _mockAttractionService.Setup(a => a.SetAttractionStatus(attraction, AttractionStatus.Open)).Returns(true);

            // Act
            var result = _engine.TryChangeAttractionStatus(attraction, AttractionStatus.Maintenance, restartMinutes: 1);

            // Assert
            Assert.True(result);
            _mockAttractionService.Verify(a => a.ClearVisitorsFromAttraction(attraction), Times.Once);
        }
    }
}
