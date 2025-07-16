using Microsoft.Extensions.Configuration;
using Park2.Domain.Enums;
using Park2.Domain.Models;
using Park2.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Infrastructure
{
    public class ParkInitialization
    {
        public void Initialize(Park park, IConfiguration config)
        {
            var configs = config.GetSection("Attractions").Get<List<AttractionSettings>>();

            if (configs == null || configs.Count == 0)
                return;

            foreach (var cfg in configs)
            {
                var attraction = new Attraction
                {
                    Id = Guid.NewGuid(),
                    Name = cfg.Name,
                    Capacity = cfg.Capacity,
                    Duration = TimeSpan.FromSeconds(cfg.DurationSeconds),
                    MinAge = cfg.MinAge,
                    TicketPrice = cfg.TicketPrice,
                    Status = Enum.Parse<AttractionStatus>(cfg.Status),
                    OccupiedSlots = new(),
                    TotalVisitors = 0,
                    MaxQueueLength = cfg.MaxQueueLength,
                    WaitTimes = new(),
                    VipQueue = new(),
                    RegularQueue = new()
                };

                park.Attractions.Add(attraction);
            }
        }
    }
}
