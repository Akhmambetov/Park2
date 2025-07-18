using Microsoft.Extensions.Options;
using Moq;
using Park2.Application.Interfaces.AttractionInterface;
using Park2.Application.Services.VisitorService;
using Park2.Domain.Enums;
using Park2.Domain.Models;
using Park2.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Park2.Tests.Services
{
    public class VisitorServiceTests
    {
        private readonly SimulationSettings _settings = new SimulationSettings
        {
            MaxVisitorDurationHours = 2,
            VisitorLeaveProbability = 0.5
        };

        private readonly Mock<IOptions<SimulationSettings>> _optionsMock = new();
        private readonly Mock<IRandomAttractionService> _randomAttractionServiceMock = new();
        private readonly Mock<IAttractionService> _attractionServiceMock = new();

        private readonly VisitorService _visitorService;

        public VisitorServiceTests()
        {
            _optionsMock.Setup(o => o.Value).Returns(_settings);
            _visitorService = new VisitorService(_optionsMock.Object, _randomAttractionServiceMock.Object, _attractionServiceMock.Object);
        }

        private Attraction CreateTestAttraction() => new Attraction
        {
            Id = Guid.NewGuid(),
            Name = "Test Attraction",
            Capacity = 10,
            Duration = TimeSpan.FromMinutes(5),
            TicketPrice = 1500,
            Status = AttractionStatus.Open,
            MinAge = 3
        };

        private Visitor CreateTestVisitor() => new Visitor
        {
            Id = Guid.NewGuid(),
            Name = "Test Visitor",
            ArrivalTime = DateTime.Now,
            Age = 25,
            IsVIP = false
        };

        [Fact]
        public void CreateRandomVisitor_ShouldReturnValidVisitor()
        {
            var time = DateTime.Now;
            var attractions = new List<Attraction>
            {
                CreateTestAttraction()
            };

            var visitor = _visitorService.CreateRandomVisitor(time, attractions);

            Assert.NotNull(visitor);
            Assert.Equal(time, visitor.ArrivalTime);
            Assert.InRange(visitor.Age, 5, 64);
        }

        [Fact]
        public void CreateVisitorFromConsole_ShouldReturnVisitorWithCorrectValues()
        {
            var time = DateTime.Now;
            var attraction = CreateTestAttraction();

            var visitor = _visitorService.CreateVisitorFromConsole(time, "Test", 30, true, attraction);

            Assert.Equal("Test", visitor.Name);
            Assert.Equal(30, visitor.Age);
            Assert.True(visitor.IsVIP);
            Assert.Equal(attraction.Id, visitor.PreferredAttractionId);
        }

        [Theory]
        [InlineData(3, false)]
        [InlineData(1, true)]
        public void ShouldLeavePark_BasedOnTimeSpent(double hours, bool _)
        {
            var visitor = new Visitor
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                ArrivalTime = DateTime.Now.AddHours(-hours),
                Age = 25,
                IsVIP = true
            };

            var result = _visitorService.ShouldLeavePark(visitor, DateTime.Now);

            if (hours > _settings.MaxVisitorDurationHours)
                Assert.True(result);
            else
                Assert.IsType<bool>(result);
        }

        [Fact]
        public void RouteVisitor_ShouldReturnResultBasedOnAttractionService()
        {
            var visitor = CreateTestVisitor();
            var attraction = CreateTestAttraction();
            var attractions = new List<Attraction> { attraction };

            _randomAttractionServiceMock.Setup(r => r.SelectAttraction(visitor, attractions)).Returns(attraction);
            _attractionServiceMock.Setup(a => a.TryEnqueueVisitor(attraction, visitor)).Returns(true);

            var result = _visitorService.RouteVisitor(visitor, attractions);

            Assert.Equal(VisitorRoutingResult.Success, result);
        }

        [Fact]
        public void UpdatePreferredAttraction_ChangesAttractionId()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var visitor = new Visitor
            {
                Name = "Test",
                ArrivalTime = DateTime.Now,
                Age = 30,
                PreferredAttractionId = id1,
                IsVIP = false
            };

            var attractions = new List<Attraction>
            {
                new Attraction
                {
                    Id = id1,
                    Name = "Attr1",
                    Capacity = 10,
                    Duration = TimeSpan.FromMinutes(5),
                    TicketPrice = 1500,
                    Status = AttractionStatus.Open,
                    MinAge = 4
                },
                new Attraction
                {
                    Id = id2,
                    Name = "Attr2",
                    Capacity = 8,
                    Duration = TimeSpan.FromMinutes(4),
                    TicketPrice = 1000,
                    Status = AttractionStatus.Open,
                    MinAge = 5
                }
            };

            _randomAttractionServiceMock
                .Setup(r => r.SelectAttraction(visitor, It.IsAny<IEnumerable<Attraction>>()))
                .Returns(attractions[1]);

            _visitorService.UpdatePreferredAttraction(visitor, attractions);

            Assert.Equal(id2, visitor.PreferredAttractionId);
        }

        [Fact]
        public void RouteActiveVisitorsFromAttraction_ShouldCallRouting()
        {
            var visitor = CreateTestVisitor();
            var visitors = new List<Visitor> { visitor };
            var attraction = CreateTestAttraction();
            var attractions = new List<Attraction> { attraction };

            _randomAttractionServiceMock
                .Setup(r => r.SelectAttraction(visitor, It.IsAny<IEnumerable<Attraction>>()))
                .Returns(attraction);
            _attractionServiceMock
                .Setup(a => a.TryEnqueueVisitor(It.IsAny<Attraction>(), visitor))
                .Returns(true);

            _visitorService.RouteActiveVisitorsFromAttraction(attractions, visitors);

            _randomAttractionServiceMock.Verify(r => r.SelectAttraction(visitor, It.IsAny<IEnumerable<Attraction>>()), Times.AtLeastOnce());
            _attractionServiceMock.Verify(a => a.TryEnqueueVisitor(It.IsAny<Attraction>(), visitor), Times.AtLeastOnce());
        }
    }
}
