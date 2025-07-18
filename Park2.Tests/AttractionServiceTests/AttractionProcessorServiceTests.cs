using Microsoft.Extensions.Logging;
using Moq;
using Org.BouncyCastle.Utilities;
using Park2.Application.Interfaces;
using Park2.Application.Interfaces.ReportInterfaces;
using Park2.Application.Interfaces.VisitorInterface;
using Park2.Application.Services.AttrationService;
using Park2.Domain.Enums;
using Park2.Domain.Models;
using Times = Moq.Times;

namespace Park2.Tests.AttractionServiceTests
{
    public class AttractionProcessorServiceTests
    {
        [Fact]
        public async void Test1()
        {
            // Arrange
            var clockMock = new Mock<IClockSimulationService>();
            var visitorServiceMock = new Mock<IVisitorService>();
            var reportServiceMock = new Mock<IReportService>();
            var loggerMock = new Mock<ILogger<AttractionProcessorService>>();

            var now = DateTime.Now;
            clockMock.Setup(c => c.CurrentTime).Returns(() => now);

            var park = new Park();
            var attraction = new Attraction
            {
                Id = Guid.NewGuid(),
                Name = "Test Ride",
                Capacity = 2,
                Duration = TimeSpan.FromMilliseconds(10),
                Status = AttractionStatus.Open,
                MinAge = 3,
                TicketPrice = 500
            };

            var visitor1 = new Visitor
            {
                Name = "Alice",
                QueueEnqueuedAt = now.AddMinutes(-5),
                IsVIP = false,
                ArrivalTime = now.AddMinutes(-10),
                Age = 18
            };
            var visitor2 = new Visitor
            {
                Name = "Bob",
                QueueEnqueuedAt = now.AddMinutes(-3),
                IsVIP = false,
                ArrivalTime = now.AddMinutes(-10),
                Age = 20
            };

            attraction.RegularQueue.Enqueue(visitor1);
            attraction.RegularQueue.Enqueue(visitor2);

            park.Attractions.Add(attraction);
            park.ActiveVisitors.AddRange(new[] { visitor1, visitor2 });

            reportServiceMock
                .Setup(r => r.GetMoneyFromAttractionLaunch(It.IsAny<Guid>(), It.IsAny<int>()))
                .Returns(1000);

            var service = new AttractionProcessorService(
                park,
                clockMock.Object,
                visitorServiceMock.Object,
                loggerMock.Object,
                reportServiceMock.Object);

            using var cts = new CancellationTokenSource();

            // Act
            var processingTask = service.ProcessQueueAsync(attraction, cts.Token);
            await Task.Delay(200); // ??????? ?????????? ?????? ???????
            cts.Cancel();
            await processingTask;
            

            // Assert
            Assert.Equal(2, attraction.TotalVisitors);
            reportServiceMock.Verify(r => r.IncreaseTotalRevenue(1000), Times.Once);
            visitorServiceMock.Verify(v => v.RouteVisitor(It.IsAny<Visitor>(), It.IsAny<List<Attraction>>()), Times.Exactly(2));
        }
    }
}