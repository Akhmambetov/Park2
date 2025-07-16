using Park2.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Application.Services
{
    public class ClockSimulationService : IClockSimulationService
    {
        public DateTime CurrentTime { get; private set; } = DateTime.Today;
        private double _speedMultiplier = 1.0;
        public event Action<DateTime>? OnTimeTick;

        public void SetStartTime(DateTime startTime)
        {
            CurrentTime = startTime;
        }
        public void Start(TimeSpan stepInterval, CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay((int)(stepInterval.TotalMilliseconds / _speedMultiplier), token);
                    CurrentTime = CurrentTime.AddMinutes(1);
                    OnTimeTick?.Invoke(CurrentTime);
                }
            }, token);
        }

        public void SetSpeedMultiplier(double multiplier)
        {
            _speedMultiplier = Math.Max(0.1, multiplier);
        }
    }
}
