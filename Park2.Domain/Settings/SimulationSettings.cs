using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Domain.Settings
{
    public class SimulationSettings
    {
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public int MinuteDurationMs { get; set; }
        public int MaxVisitors { get; set; }
        public int MaxVisitorDurationHours { get; set; }
        public double VisitorLeaveProbability { get; set; }
    }
}
