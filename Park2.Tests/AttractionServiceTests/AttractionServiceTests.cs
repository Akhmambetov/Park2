using Moq;
using Park2.Application.Interfaces;
using Park2.Application.Services.AttrationService;
using Park2.Domain.Enums;
using Park2.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xunit;

namespace Park2.Tests.Services
{
    public class AttractionServiceTests
    {
        private readonly Mock<IClockSimulationService> _mockClock;
        private readonly AttractionService _service;
        private readonly DateTime _fixedTime = new(2025, 1, 1, 14, 0, 0);

        public AttractionServiceTests()
        {
            _mockClock = new Mock<IClockSimulationService>();
            _mockClock.Setup(c => c.CurrentTime).Returns(_fixedTime);

            _service = new AttractionService(_mockClock.Object);
        }

        private Attraction CreateAttraction()
        {
            return new Attraction
            {
                Name = "Test Attraction",
                Status = AttractionStatus.Open,
                MinAge = 5,
                RegularQueue = new ConcurrentQueue<Visitor>(),
                VipQueue = new ConcurrentQueue<Visitor>(),
                OccupiedSlots = new List<Visitor>(),
                Capacity = 10,                          
                Duration = TimeSpan.FromMinutes(5),     
                TicketPrice = 1000,
            };
        }

        private Visitor CreateVisitor(int age = 10, bool isVip = false)
        {
            return new Visitor
            {
                Id = Guid.NewGuid(),
                Age = age,
                IsVIP = isVip,
                Name = "Test Visitor",
                ArrivalTime = DateTime.Today.AddHours(14)
            };
        }

        [Fact]
        public void TryEnqueueVisitor_Should_ReturnFalse_When_Underage()
        {
            var attraction = CreateAttraction();
            var visitor = CreateVisitor(age: 3);

            var result = _service.TryEnqueueVisitor(attraction, visitor);

            Assert.False(result);
        }

        [Fact]
        public void TryEnqueueVisitor_Should_ReturnFalse_When_AttractionClosed()
        {
            var attraction = CreateAttraction();
            attraction.Status = AttractionStatus.Closed;

            var visitor = CreateVisitor();

            var result = _service.TryEnqueueVisitor(attraction, visitor);

            Assert.False(result);
        }

        [Fact]
        public void TryEnqueueVisitor_Should_EnqueueToVipQueue_When_VisitorIsVip()
        {
            var attraction = CreateAttraction();
            var visitor = CreateVisitor(isVip: true);

            var result = _service.TryEnqueueVisitor(attraction, visitor);

            Assert.True(result);
            Assert.Single(attraction.VipQueue);
            Assert.Equal(_fixedTime, visitor.QueueEnqueuedAt);
        }

        [Fact]
        public void TryEnqueueVisitor_Should_EnqueueToRegularQueue_When_NotVip()
        {
            var attraction = CreateAttraction();
            var visitor = CreateVisitor(isVip: false);

            var result = _service.TryEnqueueVisitor(attraction, visitor);

            Assert.True(result);
            Assert.Single(attraction.RegularQueue);
            Assert.Equal(_fixedTime, visitor.QueueEnqueuedAt);
        }

        [Fact]
        public void TryEnqueueVisitor_Should_ReturnFalse_IfAlreadyQueued()
        {
            var attraction = CreateAttraction();
            var visitor = CreateVisitor();
            attraction.RegularQueue.Enqueue(visitor);

            var result = _service.TryEnqueueVisitor(attraction, visitor);

            Assert.False(result);
        }

        [Fact]
        public void IsVisitorQueued_Should_ReturnTrue_If_InAnyQueue()
        {
            var attraction = CreateAttraction();
            var visitor = CreateVisitor();
            attraction.VipQueue.Enqueue(visitor);

            var result = _service.IsVisitorQueued(attraction, visitor.Id);

            Assert.True(result);
        }

        [Fact]
        public void ClearVisitorsFromAttraction_Should_ClearAllQueuesAndSlots()
        {
            var attraction = CreateAttraction();
            var visitor = CreateVisitor();
            attraction.VipQueue.Enqueue(visitor);
            attraction.RegularQueue.Enqueue(visitor);
            attraction.OccupiedSlots.Add(visitor);

            _service.ClearVisitorsFromAttraction(attraction);

            Assert.Empty(attraction.VipQueue);
            Assert.Empty(attraction.RegularQueue);
            Assert.Empty(attraction.OccupiedSlots);
        }

        [Fact]
        public void SetAttractionStatus_Should_ChangeStatus_IfDifferent()
        {
            var attraction = CreateAttraction();
            attraction.Status = AttractionStatus.Open;

            var result = _service.SetAttractionStatus(attraction, AttractionStatus.Maintenance);

            Assert.True(result);
            Assert.Equal(AttractionStatus.Maintenance, attraction.Status);
        }

        [Fact]
        public void SetAttractionStatus_Should_ReturnFalse_IfSameStatus()
        {
            var attraction = CreateAttraction();
            attraction.Status = AttractionStatus.Closed;

            var result = _service.SetAttractionStatus(attraction, AttractionStatus.Closed);

            Assert.False(result);
        }
    }
}
