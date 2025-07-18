using Park2.Application.Interfaces.AttractionInterface;
using Park2.Domain.Enums;
using Park2.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Application.Services.AttrationService
{
    public class RandomAttractionService : IRandomAttractionService
    {
        public Attraction? SelectAttraction(Visitor visitor, IEnumerable<Attraction> attractions)
        {
            var available = attractions
                .Where(a => a.Status == AttractionStatus.Open &&
                            (a.MinAge == 0 || visitor.Age >= a.MinAge) &&
                            a.CurrentQueueLength < a.MaxQueueLength &&
                            a.OccupiedSlotsCount < a.Capacity)
                .ToList();

            if (!available.Any())
                return null;

            if (visitor.PreferredAttractionId != null)
            {
                var preferred = available.FirstOrDefault(a =>
                    a.Id == visitor.PreferredAttractionId);

                if (preferred != null)
                    return preferred;
            }

            return available[Random.Shared.Next(available.Count)];
        }
    }
}
