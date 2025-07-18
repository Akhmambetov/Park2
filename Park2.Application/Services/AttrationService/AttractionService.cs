using Microsoft.Extensions.Options;
using Park2.Application.Interfaces;
using Park2.Application.Interfaces.AttractionInterface;
using Park2.Domain.Enums;
using Park2.Domain.Models;
using Park2.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Application.Services.AttrationService
{
    public class AttractionService : IAttractionService
    {
        private readonly IClockSimulationService _clock;

        public AttractionService(IClockSimulationService clock)
        {
            _clock = clock;
        }

        public bool TryEnqueueVisitor(Attraction attraction, Visitor visitor)
        {
            if (visitor.Age < attraction.MinAge) 
                return false;

            if (attraction.Status != AttractionStatus.Open)
                return false;

            if (IsVisitorQueued(attraction, visitor.Id))
                return false;

            if (visitor.IsVIP)
            {
                visitor.QueueEnqueuedAt = _clock.CurrentTime;
                attraction.VipQueue.Enqueue(visitor);
            }
            else
            {
                visitor.QueueEnqueuedAt = _clock.CurrentTime;
                attraction.RegularQueue.Enqueue(visitor);
            }

            int currentLength = attraction.CurrentQueueLength;
            if (currentLength > attraction.MaxQueueLength)
                attraction.MaxQueueLength = currentLength;

            return true;
        }

        public bool IsVisitorQueued(Attraction attraction, Guid visitorId)
        {
            return attraction.VipQueue.Any(v => v.Id == visitorId)
                || attraction.RegularQueue.Any(v => v.Id == visitorId);
        }

        public void ClearVisitorsFromAttraction(Attraction attraction)
        {
            attraction.VipQueue.Clear();
            attraction.RegularQueue.Clear();

            attraction.OccupiedSlots.Clear();
        }

        public bool SetAttractionStatus(Attraction attraction, AttractionStatus newStatus)
        {
            if (attraction.Status == newStatus)
                return false;

            attraction.Status = newStatus;
            return true;
        }
    }
}
