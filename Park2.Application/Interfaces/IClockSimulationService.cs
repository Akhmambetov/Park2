using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Application.Interfaces
{
    public interface IClockSimulationService
    {
        DateTime CurrentTime { get; }
        void SetStartTime(DateTime startTime);
        void Start(TimeSpan stepInterval, CancellationToken token);
        void SetSpeedMultiplier(double multiplier);
        event Action<DateTime> OnTimeTick;
    }
}
