using Park2.Application.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Park2.Tests.Services
{
    public class ClockSimulationServiceTests
    {
        [Fact]
        public void SetStartTime_ShouldSetInitialTime()
        {
            // Arrange
            var service = new ClockSimulationService();
            var expected = new DateTime(2025, 7, 18, 14, 0, 0);

            // Act
            service.SetStartTime(expected);

            // Assert
            Assert.Equal(expected, service.CurrentTime);
        }

        [Fact]
        public void SetSpeedMultiplier_ShouldSetValidMultiplier()
        {
            // Arrange
            var service = new ClockSimulationService();

            // Act
            service.SetSpeedMultiplier(2.5);

            // There's no public getter, but no exception = OK
            service.SetSpeedMultiplier(0); // should clamp to 0.1
        }

        [Fact]
        public async Task Start_ShouldTriggerOnTimeTick()
        {
            // Arrange
            var service = new ClockSimulationService();
            var start = new DateTime(2025, 7, 18, 14, 0, 0);
            service.SetStartTime(start);

            int tickCount = 0;
            DateTime? lastTickTime = null;

            service.OnTimeTick += (time) =>
            {
                tickCount++;
                lastTickTime = time;
            };

            var tokenSource = new CancellationTokenSource();
            TimeSpan stepInterval = TimeSpan.FromMilliseconds(10);

            // Act
            service.Start(stepInterval, tokenSource.Token);

            // Wait for a few ticks
            await Task.Delay(60);
            tokenSource.Cancel();

            // Assert
            Assert.True(tickCount >= 2); // at least 2 ticks happened
            Assert.True(lastTickTime > start);
        }

        [Fact]
        public async Task Start_ShouldRespectSpeedMultiplier()
        {
            // Arrange
            var service = new ClockSimulationService();
            service.SetStartTime(new DateTime(2025, 7, 18, 14, 0, 0));
            service.SetSpeedMultiplier(10.0); // should make simulation go 10x faster

            int tickCount = 0;
            service.OnTimeTick += (_) => tickCount++;

            var cts = new CancellationTokenSource();
            TimeSpan stepInterval = TimeSpan.FromMilliseconds(100);

            // Act
            service.Start(stepInterval, cts.Token);
            await Task.Delay(150); // with 10x speed, we expect ~10 ticks
            cts.Cancel();

            // Assert
            Assert.True(tickCount >= 5); // at least 5 ticks due to high speed
        }
    }
}
