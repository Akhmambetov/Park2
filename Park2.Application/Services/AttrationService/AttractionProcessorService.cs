using Microsoft.Extensions.Logging;
using Park2.Application.Interfaces;
using Park2.Application.Interfaces.VisitorInterface;
using Park2.Domain.Enums;
using Park2.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Application.Services.AttrationService
{
    public class AttractionProcessorService
    {
        private readonly Park _park;
        private readonly IClockSimulationService _clock;
        private readonly IVisitorService _visitorService;
        private readonly ILogger<AttractionProcessorService> _logger;

        public AttractionProcessorService(
            Park park,
            IClockSimulationService clock,
            IVisitorService visitorService,
            ILogger<AttractionProcessorService> logger)
        {
            _park = park;
            _clock = clock;
            _visitorService = visitorService;
            _logger = logger;
        }

        public async Task ProcessQueueAsync(Attraction attraction, CancellationToken token)
        {
            //_logger.LogInformation("Start processing queue for attraction: {Name}", attraction.Name);

            DateTime? firstEnqueueTime = null;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (attraction.Status != AttractionStatus.Open)
                    {
                        await Task.Delay(100, token);
                        continue;
                    }

                    List<Visitor> visitors = new();
                    bool shouldStartRide = false;
                    int vipCount = attraction.VipQueue.Count;
                    int regularCount = attraction.RegularQueue.Count;
                    int totalQueue = vipCount + regularCount;

                    if (totalQueue == 0)
                    {
                        firstEnqueueTime = null;
                    }
                    else if (firstEnqueueTime == null)
                    {
                        firstEnqueueTime = _clock.CurrentTime;
                    }

                    shouldStartRide =
                            totalQueue >= attraction.Capacity ||
                            (firstEnqueueTime.HasValue && _clock.CurrentTime - firstEnqueueTime >= TimeSpan.FromMinutes(15));

                    if (shouldStartRide)
                    {
                        List<string> vipNames = new();
                        List<string> regularNames = new();

                        while (visitors.Count < attraction.Capacity && attraction.VipQueue.TryDequeue(out var vip))
                        {
                            visitors.Add(vip);
                            vipNames.Add(vip.Name);
                        }

                        while (visitors.Count < attraction.Capacity && attraction.RegularQueue.TryDequeue(out var regular))
                        {
                            visitors.Add(regular);
                            regularNames.Add(regular.Name);
                        }
                        attraction.OccupiedSlots.AddRange(visitors);
                        attraction.TotalVisitors += visitors.Count;

                        // Надо разобраться с Wait Time

                        _logger.LogInformation(
                            "Ride starting on {Attraction}. VIPs: [{VIPs}], Regulars: [{Regulars}]",
                            attraction.Name,
                            string.Join(", ", vipNames),
                            string.Join(", ", regularNames));

                        //_logger.LogInformation("🎢 Start Attraction {Name}. Amount of Visitors: {Count}. Duration: {Duration}. Queue: {Queue}",
                        //    attraction.Name,
                        //    visitors.Count,
                        //    attraction.Duration,
                        //    attraction.CurrentQueueLength);

                        await HandleGroupRideAsync(attraction, visitors, token);

                        await Task.Delay(100, token);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Queue processing cancelled for attraction: {Name}", attraction.Name);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing attraction: {Name}", attraction.Name);
                    await Task.Delay(500, token);
                }
            }

            _logger.LogInformation("Stopped processing queue for attraction: {Name}", attraction.Name);
        }

        private async Task HandleGroupRideAsync(Attraction attraction, List<Visitor> visitors, CancellationToken token)
        {
            try
            {
                await Task.Delay(attraction.Duration, token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            lock (attraction)
            {
                attraction.OccupiedSlots.Clear();
            }

            List<string> exitedVisitors = new();
            List<string> reroutedVisitors = new();

            foreach (var visitor in visitors)
            {
                if (_visitorService.ShouldLeavePark(visitor, _clock.CurrentTime))
                {
                    lock (_park)
                    {
                        _park.ActiveVisitors.Remove(visitor);
                        _park.DepartedVisitors.Add(visitor);
                    }

                    exitedVisitors.Add(visitor.Name);
                }
                else
                {
                    _visitorService.UpdatePreferredAttraction(visitor, _park.Attractions); // Обновление предпочитаемого аттрациона

                    _visitorService.RouteVisitor(visitor, _park.Attractions);
                    reroutedVisitors.Add(visitor.Name);
                }
            }

            _logger.LogInformation(
                "Ride completed at attraction '{AttractionName}' | Occupied slots: {Occupied}/{Capacity}\n" +
                "Visitors who left the park: {Exited}\n" +
                "Visitors who continued to another attraction: {Rerouted}",
                attraction.Name,
                attraction.OccupiedSlots,
                attraction.Capacity,
                exitedVisitors.Any() ? string.Join(", ", exitedVisitors) : "None",
                reroutedVisitors.Any() ? string.Join(", ", reroutedVisitors) : "None"
            );
        }
    }
}
