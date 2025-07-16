using Park2.Domain.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Domain.Models
{
    public class Attraction
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required TimeSpan Duration { get; set; }
        public required int MinAge { get; set; }
        public required decimal TicketPrice { get; set; }
        public ConcurrentQueue<Visitor> VipQueue { get; set; } = new ();
        public ConcurrentQueue<Visitor> RegularQueue { get; set; } = new();
        public int CurrentQueueLength => VipQueue.Count + RegularQueue.Count;
        public List<Visitor> OccupiedSlots { get; set; } = new();
        public int OccupiedSlotsCount => OccupiedSlots?.Count ?? 0;
        public required AttractionStatus Status { get; set; }
        public int MaxQueueLength { get; set; }
        public int TotalVisitors { get; set; }
        public List<TimeSpan> WaitTimes { get; set; } = new();
        public required int Capacity { get; set; }
    }
}
