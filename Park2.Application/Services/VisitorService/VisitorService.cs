using Microsoft.Extensions.Options;
using Park2.Application.Interfaces.AttractionInterface;
using Park2.Application.Interfaces.VisitorInterface;
using Park2.Application.Services.AttrationService;
using Park2.Domain.Enums;
using Park2.Domain.Models;
using Park2.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Application.Services.VisitorService
{
    public class VisitorService : IVisitorService
    {
        private readonly SimulationSettings _settings;
        private readonly IRandomAttractionService _randomAttractionService;
        private readonly IAttractionService _attractionService;

        public VisitorService(IOptions<SimulationSettings> options, IRandomAttractionService randomAttractionService, IAttractionService attractionService)
        {
            _settings = options.Value; 
            _randomAttractionService = randomAttractionService;
            _attractionService = attractionService;
        }

        public Visitor CreateRandomVisitor(DateTime currentTime, IEnumerable<Attraction> attractions)
        {
            var availableAttractions = attractions
            .Where(a => a.Status == AttractionStatus.Open)
            .ToList();

            Guid? preferredId = null;

            if (availableAttractions.Any() && Random.Shared.NextDouble() < 0.5)
            {
                var randomAttraction = availableAttractions[Random.Shared.Next(availableAttractions.Count)];
                preferredId = randomAttraction.Id;
            }

            return new Visitor
            {
                Id = Guid.NewGuid(),
                Name = $"Visitor_{Random.Shared.Next(1000)}",
                ArrivalTime = currentTime,
                Age = Random.Shared.Next(5, 65),
                IsVIP = Random.Shared.NextDouble() < 0.15,
                PreferredAttractionId = preferredId
            };
        }

        public Visitor CreateVisitorFromConsole(
            DateTime currentTime,
            string name,
            int age,
            bool isVIP,
            Attraction chosenAttraction)
        {
            var visitor = new Visitor
            {
                Id = Guid.NewGuid(),
                Name = name,
                Age = age,
                IsVIP = isVIP,
                ArrivalTime = currentTime,
                PreferredAttractionId = chosenAttraction.Id
            };

            return visitor; // 👈 без очереди
        }


        public bool ShouldLeavePark(Visitor visitor, DateTime currentTime)
        {
            var timeSpentInPark = currentTime - visitor.ArrivalTime;

            if (timeSpentInPark.TotalHours > _settings.MaxVisitorDurationHours)
            {
                return true;
            }

            bool decidesToLeave = Random.Shared.NextDouble() < _settings.VisitorLeaveProbability;

            return decidesToLeave;
        }

        public VisitorRoutingResult RouteVisitor(Visitor visitor, IEnumerable<Attraction> attractions)
        {
            var chosen = _randomAttractionService.SelectAttraction(visitor, attractions);
            if (chosen == null)
                return VisitorRoutingResult.NoAvailableAttractions;

            bool enqueued = _attractionService.TryEnqueueVisitor(chosen, visitor);
            return enqueued ? VisitorRoutingResult.Success : VisitorRoutingResult.EnqueueFailed;
        }

        public void UpdatePreferredAttraction(Visitor visitor, IEnumerable<Attraction> attractions)
        {
            // Фильтруем доступные аттракционы, исключая текущий предпочтительный
            var eligibleAttractions = attractions
                .Where(a =>
                    a.Status == AttractionStatus.Open &&
                    a.Id != visitor.PreferredAttractionId)
                .ToList();

            if (!eligibleAttractions.Any())
                return;

            // Выбираем новый предпочтительный аттракцион случайным образом
            var newPreferred = _randomAttractionService.SelectAttraction(visitor, eligibleAttractions);

            if (newPreferred != null)
                visitor.PreferredAttractionId = newPreferred.Id;
        }

        public void RouteActiveVisitorsFromAttraction(List<Attraction> attractions, List<Visitor> visitors)
        {
            if (visitors != null)
            {
                foreach (var visitor in visitors)
                {
                    UpdatePreferredAttraction(visitor, attractions);
                    RouteVisitor(visitor, attractions);
                }
            }
        }
    }
}
