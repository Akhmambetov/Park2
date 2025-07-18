using Park2.Application.Services.AttrationService;
using Park2.Domain.Enums;
using Park2.Domain.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace Park2.Tests.Services
{
    public class RandomAttractionServiceTests
    {
        private readonly RandomAttractionService _service = new();

        private static Attraction CreateAttraction(
            Guid? id = null,
            AttractionStatus status = AttractionStatus.Open,
            int minAge = 0,
            int capacity = 10,
            int occupied = 0,
            int maxQueueLength = 5,
            int vipQueueLength = 0,
            int regularQueueLength = 0)
        {
            var attraction = new Attraction
            {
                Id = id ?? Guid.NewGuid(),
                Name = "Test Attraction",
                Status = status,
                MinAge = minAge,
                Capacity = capacity,
                Duration = TimeSpan.FromMinutes(10),
                TicketPrice = 1000,
                MaxQueueLength = maxQueueLength
            };

            // Добавляем посетителей в очереди
            for (int i = 0; i < vipQueueLength; i++)
                attraction.VipQueue.Enqueue(CreateVisitor());

            for (int i = 0; i < regularQueueLength; i++)
                attraction.RegularQueue.Enqueue(CreateVisitor());

            return attraction;
        }


        private static Visitor CreateVisitor(int age = 20, Guid? preferredId = null)
        {
            return new Visitor
            {
                Id = Guid.NewGuid(),
                Age = age,
                Name = "Test Visitor",
                ArrivalTime = DateTime.Today.AddHours(14),
                PreferredAttractionId = preferredId,
                IsVIP = true
            };
        }

        [Fact]
        public void SelectAttraction_ReturnsPreferred_WhenAvailable()
        {
            // Arrange
            var preferredId = Guid.NewGuid();
            var preferred = CreateAttraction(preferredId, occupied: 0);
            var other = CreateAttraction();
            var visitor = CreateVisitor(preferredId: preferredId);
            var attractions = new List<Attraction> { preferred, other };

            // Act
            var selected = _service.SelectAttraction(visitor, attractions);

            // Assert
            Assert.Equal(preferredId, selected?.Id);
        }

        [Fact]
        public void SelectAttraction_ReturnsRandom_WhenPreferredFull()
        {
            // Arrange
            var preferredId = Guid.NewGuid();

            var preferred = CreateAttraction(
                id: preferredId,
                capacity: 5,
                occupied: 5,
                maxQueueLength: 3,
                vipQueueLength: 2,
                regularQueueLength: 1 // Полная очередь
            );

            var available = CreateAttraction(); // Свободный аттракцион

            var visitor = CreateVisitor(preferredId: preferredId);
            var attractions = new List<Attraction> { preferred, available };

            // Act
            var selected = _service.SelectAttraction(visitor, attractions);

            // Assert
            Assert.NotEqual(preferredId, selected?.Id);
        }

        [Fact]
        public void SelectAttraction_ReturnsNull_WhenNoneAvailable()
        {
            // Arrange
            var visitor = CreateVisitor(age: 10); // Маленький возраст

            var closed = CreateAttraction(status: AttractionStatus.Maintenance); // Закрыт

            var tooYoung = CreateAttraction(minAge: 21); // Не проходит по возрасту

            var full = CreateAttraction(maxQueueLength: 2, vipQueueLength: 1, regularQueueLength: 1); // Очередь полная

            var attractions = new List<Attraction> { closed, tooYoung, full };

            // Act
            var selected = _service.SelectAttraction(visitor, attractions);

            // Assert
            Assert.Null(selected);
        }

        [Fact]
        public void SelectAttraction_RespectsMinAge()
        {
            // Arrange
            var visitor = CreateVisitor(age: 5);
            var adultRide = CreateAttraction(minAge: 18);
            var childRide = CreateAttraction(minAge: 3);
            var attractions = new List<Attraction> { adultRide, childRide };

            // Act
            var selected = _service.SelectAttraction(visitor, attractions);

            // Assert
            Assert.Equal(childRide.Id, selected?.Id);
        }
    }
}
