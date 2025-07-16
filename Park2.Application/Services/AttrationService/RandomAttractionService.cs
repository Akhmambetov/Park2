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
            // Все доступные аттракционы по возрасту и статусу
            var available = attractions
                .Where(a => a.Status == AttractionStatus.Open &&
                            (a.MinAge == 0 || visitor.Age >= a.MinAge))
                .ToList();

            if (!available.Any())
                return null;

            // Попытка использовать предпочитаемый аттракцион
            if (visitor.PreferredAttractionId != null)
            {
                var preferred = available.FirstOrDefault(a =>
                    a.Id == visitor.PreferredAttractionId &&
                    a.OccupiedSlotsCount < a.Capacity);

                if (preferred != null)
                    return preferred;
            }

            // Если preferred недоступен — выбрать случайный доступный
            var candidates = available
                .Where(a => a.OccupiedSlotsCount < a.Capacity)
                .ToList();

            if (!candidates.Any())
                return null;

            return candidates[Random.Shared.Next(candidates.Count)];
        }
    }
}
