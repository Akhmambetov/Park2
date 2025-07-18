using Microsoft.Extensions.Options;
using Park2.Application.Interfaces;
using Park2.Application.Simulation;
using Park2.Domain.Models;
using Park2.Domain.Settings;
using Park2.Presentation.ConsoleUI.AttractionPrinter;
using Park2.Presentation.ConsoleUI.Menu;
using Park2.Presentation.ConsoleUI.Report;
using Park2.Presentation.ConsoleUI.VisitorPrinter;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Park2.Presentation.ConsoleUI
{
    public class ConsoleApp
    {
        private readonly IClockSimulationService _clock;
        private readonly SimulationSettings _settings;
        private readonly Park _park;
        private readonly SimulationEngine _engine;
        private readonly IReportController _reportController;
        private readonly IMenuPrinter _menuPrinter;
        private readonly IReportMenuPrinter _reportMenuPrinter;
        private readonly IVisitorCreator _visitorCreator;
        private readonly IVisitorStatsPrinter _visitorStatsPrinter;
        private readonly IAttractionStatusPrinter _attractionStatusPrinter;
        private readonly IConsoleChangeAttractionStatus _consoleChangeAttractionStatus;
        private readonly IInputReader _reader;
        private readonly IOutputWriter _writer;

        private DateTime _simulatedTime;
        private readonly CancellationTokenSource _cts;

        public ConsoleApp(
            IClockSimulationService clock,
            IOptions<SimulationSettings> settings,
            Park park,
            SimulationEngine engine,
            IReportController reportController,
            IMenuPrinter menuPrinter,
            IReportMenuPrinter reportMenuPrinter,
            IVisitorCreator visitorCreator,
            IVisitorStatsPrinter visitorStatsPrinter,
            IAttractionStatusPrinter attractionStatusPrinter,
            IConsoleChangeAttractionStatus consoleChangeAttractionStatus,
            IInputReader reader,
            IOutputWriter writer)
        {
            _clock = clock;
            _settings = settings.Value;
            _park = park;
            _engine = engine;
            _reportController = reportController;
            _menuPrinter = menuPrinter;
            _reportMenuPrinter = reportMenuPrinter;
            _visitorCreator = visitorCreator;
            _visitorStatsPrinter = visitorStatsPrinter;
            _attractionStatusPrinter = attractionStatusPrinter;
            _consoleChangeAttractionStatus = consoleChangeAttractionStatus;
            _reader = reader;
            _writer = writer;

            _simulatedTime = DateTime.Today.AddHours(_settings.StartHour);
            _cts = new CancellationTokenSource();
        }

        public async Task Run()
        {
            Console.OutputEncoding = Encoding.UTF8;

            _menuPrinter.ShowWelcome();
            var key = _reader.ReadKey();
            if (key.KeyChar == '1')
            {
                _writer.Clear();
                await StartSimulationAsync();
            }
        }

        private async Task StartSimulationAsync()
        {
            _writer.Clear();
            _writer.WriteLine("🔄 Запуск симуляции времени...");
            await _engine.StartAsync(_cts.Token);

            _clock.OnTimeTick += OnTimeTick;

            var tickInterval = TimeSpan.FromMilliseconds(_settings.MinuteDurationMs);
            _clock.SetStartTime(DateTime.Today.AddHours(_settings.StartHour));
            _clock.Start(tickInterval, _cts.Token);

            await ShowMainMenuLoopAsync();
        }

        private async Task ShowMainMenuLoopAsync()
        {
            while (_simulatedTime.Hour < _settings.EndHour && !_cts.IsCancellationRequested)
            {
                _menuPrinter.ShowMainMenu(_simulatedTime);
                var input = _reader.ReadKey().KeyChar;

                switch (char.ToLower(input))
                {
                    case '1':
                        _menuPrinter.ShowTime(_simulatedTime);
                        break;
                    case '2':
                        _attractionStatusPrinter.PrintStatus(_park.Attractions);
                        break;
                    case '3':
                        _visitorStatsPrinter.ShowVisitorStats(_park.ActiveVisitors, _park.DepartedVisitors, _settings.MaxVisitors);
                        break;
                    case '4':
                        _engine.StopVisitorGeneration();
                        _writer.WriteLine("\n🛑 Генерация новых посетителей остановлена.");
                        break;
                    case '5':
                        _consoleChangeAttractionStatus.ChangeAttractionStatus(_park);
                        break;
                    case '6':
                        _reportMenuPrinter.ShowReportMenu();
                        break;
                    case 'n':
                        _visitorCreator.CreateVisitorFromConsoleInput(_park.Attractions, _engine);
                        break;
                    case 'q':
                        _cts.Cancel();
                        _writer.Clear();
                        _writer.WriteLine("❌ Симуляция остановлена. До свидания!");
                        return;
                }

                _writer.WriteLine("\nНажмите любую клавишу для возврата в меню...");
                _reader.ReadKey();
            }
        }

        private void OnTimeTick(DateTime currentTime)
        {
            _simulatedTime = currentTime;
        }
    }
}
