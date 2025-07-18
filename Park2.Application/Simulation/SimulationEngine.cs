using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Park2.Application.Interfaces;
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

namespace Park2.Application.Simulation
{
    public class SimulationEngine
    {
        private readonly IClockSimulationService _clock;
        private readonly Park _park;
        private readonly IVisitorService _visitorService;
        private readonly AttractionProcessorService _processor;
        private readonly ILogger<SimulationEngine> _logger;
        private readonly SimulationSettings _settings;
        private bool _visitorGenerationEnabled = true;
        private readonly IAttractionService _attractionService;

        public SimulationEngine(
            IClockSimulationService clock,
            Park park,
            IVisitorService visitorService,
            AttractionProcessorService processor,
            ILogger<SimulationEngine> logger,
            IOptions<SimulationSettings> options,
            IAttractionService attractionService)
        {
            _clock = clock;
            _park = park;
            _visitorService = visitorService;
            _processor = processor;
            _logger = logger;
            _settings = options.Value;
            _attractionService = attractionService;
        }

        public Task StartAsync(CancellationToken token)
        {
            _clock.OnTimeTick += OnTimeTick;
            _clock.Start(TimeSpan.FromMilliseconds(_settings.MinuteDurationMs), token);

            foreach (var attraction in _park.Attractions)
            {
                _ = Task.Run(() => _processor.ProcessQueueAsync(attraction, token));
            }

            return Task.CompletedTask;
        }

        public void StopVisitorGeneration()
        {

            _visitorGenerationEnabled = false;
            _logger.LogInformation("Visitor generation has been stopped.");
        }

        public void StartVisitorGeneration()
        {
            _visitorGenerationEnabled = true;
            _logger.LogInformation("Visitor generation has been resumed.");
        }

        private void OnTimeTick(DateTime currentTime)
        {
            try
            {
                if (!_visitorGenerationEnabled)
                    return;

                if (_park.ActiveVisitors.Count >= _settings.MaxVisitors)
                    return;

                // 30% шанс добавить нового посетителя
                if (Random.Shared.NextDouble() < 0.3)
                {
                    var visitor = _visitorService.CreateRandomVisitor(currentTime,  _park.Attractions);

                    lock (_park)
                    {
                        _park.ActiveVisitors.Add(visitor);
                    }


                    _visitorService.RouteVisitor(visitor, _park.Attractions);

                }
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error during time tick at {Time}", currentTime);
            }
        }

        public void AddManualVisitor(string name, int age, bool isVIP, Attraction chosenAttraction)
        {
            var currentTime = _clock.CurrentTime;

            // Создание посетителя через VisitorService
            var visitor = _visitorService.CreateVisitorFromConsole(currentTime, name, age, isVIP, chosenAttraction);

            lock (_park)
            {
                _park.ActiveVisitors.Add(visitor);
            }

            var routingResult = _visitorService.RouteVisitor(visitor, new[] { chosenAttraction });
        }

        private void HandleAttractionClosure(Attraction attraction)
        {
            var currentVisitorsOnAttraction = attraction.OccupiedSlots;

            _attractionService.ClearVisitorsFromAttraction(attraction);

            _visitorService.RouteActiveVisitorsFromAttraction(_park.Attractions, currentVisitorsOnAttraction);
        }

        public bool TryChangeAttractionStatus(Attraction attraction, AttractionStatus newStatus, int restartMinutes = 0)
        {
            if (!_attractionService.SetAttractionStatus(attraction, newStatus))
                return false;

            if (newStatus == AttractionStatus.Closed)
            {
                HandleAttractionClosure(attraction);
            }

            if (newStatus == AttractionStatus.Maintenance && restartMinutes > 0)
            {
                HandleAttractionClosure(attraction);

                Task.Run(async () =>
                {
                    _logger.LogInformation("⏳ Attraction '{Name}' will reopen in {Minutes} minutes", attraction.Name, restartMinutes);

                    await Task.Delay(TimeSpan.FromSeconds(restartMinutes)); 

                    _attractionService.SetAttractionStatus(attraction, AttractionStatus.Open);

                    _logger.LogInformation("✅ Attraction '{Name}' status changed back to Open", attraction.Name);
                });
            }

            return true;
        }
    }
}
