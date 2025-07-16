using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Domain.Settings
{
    public class AttractionSettings
    {
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
        public int DurationSeconds { get; set; }
        public int MinAge { get; set; }
        public decimal TicketPrice { get; set; }
        public int MaxQueueLength { get; set; }
        public string Status { get; set; } = "Open";
    }
}
