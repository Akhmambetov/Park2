using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Domain.Models
{
    public class Visitor
    {
        public Guid Id { get; set; }
        required public string Name { get; set; }
        required public DateTimeOffset ArrivalTime { get; set; }
        public Guid? PreferredAttractionId { get; set; }
        required public int Age { get; set; }
        required public bool IsVIP { get; set; }
        public DateTime? QueueEnqueuedAt { get; set; }
    }
}
