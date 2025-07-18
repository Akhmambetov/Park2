using Microsoft.Extensions.Options;
using Park2.Application.Interfaces;
using Park2.Application.Interfaces.VisitorInterface;
using Park2.Application.Simulation;
using Park2.Domain.Enums;
using Park2.Domain.Models;
using Park2.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Presentation.ConsoleUI
{
    public class ConsoleApp
    {
        private readonly IClockSimulationService _clock;
        private readonly SimulationSettings _settings;
        private readonly Park _park;
        private DateTime _simulatedTime;
        private readonly CancellationTokenSource _cts;
        private readonly SimulationEngine _engine;
        private readonly ReportController _reportController;

        public ConsoleApp(
            IClockSimulationService clock,
            IOptions<SimulationSettings> settings,
            Park park,
            SimulationEngine engine,
            IVisitorService visitorService,
            ReportController reportController)
        {
            _clock = clock;
            _settings = settings.Value;
            _park = park;
            _engine = engine;
            _simulatedTime = DateTime.Today.AddHours(_settings.StartHour);
            _cts = new CancellationTokenSource();
            _reportController = reportController;
        }

        public async Task Run()
        {
            Console.OutputEncoding = Encoding.UTF8;

            ShowWelcome();
            var key = Console.ReadKey(intercept: true);
            if (key.KeyChar == '1')
            {
                Console.Clear();
                await StartSimulationAsync();
            }
        }

        private void ShowWelcome()
        {
            Console.Clear();
            Console.WriteLine("🎡 Добро пожаловать в симулятор парка аттракционов!");
            Console.WriteLine();
            Console.WriteLine("1. ▶️ Запустить симуляцию");
            Console.WriteLine();
            Console.Write("Введите номер пункта и нажмите Enter: ");
        }

        private async Task StartSimulationAsync()
        {
            Console.Clear();
            Console.WriteLine("🔄 Запуск симуляции времени...");
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
                MenuPrinter.ShowMainMenu(_simulatedTime);

                var input = Console.ReadKey(intercept: true).KeyChar;

                switch (input)
                {
                    case '1':
                        ShowTime();
                        break;
                    case '2':
                        ShowAttractionStatus();
                        break;
                    case '3':
                        ShowVisitorStats();
                        break;
                    case '4':
                        _engine.StopVisitorGeneration();
                        Console.WriteLine("\n🛑 Генерация новых посетителей остановлена.");
                        break;
                    case '5':
                        ChangeAttractionStatus();
                        break;
                    case '6':
                        ShowReportMenuAsync();
                        break;
                    case 'N':
                        CreateVisitorFromConsoleInput();
                        break;
                    case 'q':
                    case 'Q':
                        _cts.Cancel();
                        Console.Clear();
                        Console.WriteLine("❌ Симуляция остановлена. До свидания!");
                        return;
                }

                Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
                Console.ReadKey(true);
            }
        }

        private void ShowReportMenuAsync()
        {
            while (true)
            {
                MenuPrinter.ShowReportMenu(); // <-- Используем уже написанную функцию отображения

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.WriteLine($"На данный момент - ({_simulatedTime:HH:mm}), общая выручка с аттракционов составляет - {_reportController.GetTotalRevenue()} тенге)");
                        break;

                    case "2":
                        GetVisitorsForAttraction();
                        break;

                    case "3":
                        GetAverageWaitTimeForAttraction();
                        break;

                    case "4":
                        GetPeakQueueLengthForAttraction();
                        break;

                    case "5":
                        return;

                    default:
                        Console.WriteLine("❌ Неверный пункт. Попробуйте снова.");
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу для возврата к меню отчётов...");
                Console.ReadKey(true);
            }
        }

        private void ShowTime()
        {
            Console.Clear();
            Console.WriteLine("🕒 Текущее симулированное время:");
            Console.WriteLine($"   {_simulatedTime:HH:mm}");
        }

        private void ShowAttractionStatus()
        {
            Console.Clear();
            Console.WriteLine("🎢 Статус аттракционов:\n");

            foreach (var attraction in _park.Attractions)
            {
                Console.WriteLine($"- {attraction.Name}");
                Console.WriteLine($"  Статус: {attraction.Status}");
                Console.WriteLine($"  Очередь: {attraction.CurrentQueueLength} чел.");
                Console.WriteLine($"  Занято мест: {attraction.OccupiedSlotsCount}/{attraction.Capacity}");
                Console.WriteLine();
            }
        }

        private void ShowVisitorStats()
        {
            Console.Clear();
            Console.WriteLine("👥 Статистика посетителей:\n");
            int totalVisitors = _park.ActiveVisitors.Count + _park.DepartedVisitors.Count;

            Console.WriteLine($"   В парке: {_park.ActiveVisitors.Count}");
            Console.WriteLine($"   Ушли: {_park.DepartedVisitors.Count}");
            Console.WriteLine($"   Всего уникальных: {totalVisitors}/{_settings.MaxVisitors}");
            Console.WriteLine();

            Console.WriteLine("📌 Активные посетители:");
            if (_park.ActiveVisitors.Count == 0)
            {
                Console.WriteLine("   (Нет активных посетителей)");
            }
            else
            {
                foreach (var visitor in _park.ActiveVisitors) 
                {
                    Console.WriteLine($"   👤 {visitor.Name,-15} | Возраст: {visitor.Age,2} | VIP: {visitor.IsVIP} | Прибыл: {visitor.ArrivalTime:HH:mm}");
                }
            }

            Console.WriteLine();

            Console.WriteLine("🚶 Ушедшие посетители:");
            if (_park.DepartedVisitors.Count == 0)
            {
                Console.WriteLine("   (Никто ещё не ушёл)");
            }
            else
            {
                foreach (var visitor in _park.DepartedVisitors)
                {
                    Console.WriteLine($"   🛫 {visitor.Name,-15} | Возраст: {visitor.Age,2} | VIP: {visitor.IsVIP} | Прибыл: {visitor.ArrivalTime:HH:mm}");
                }
            }
        }

        private void CreateVisitorFromConsoleInput()
        {
            Console.Clear();
            Console.WriteLine("👤 Создание нового посетителя вручную:");

            Console.Write("Введите имя: ");
            var name = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                name = "Безымянный";

            Console.Write("Введите возраст: ");
            if (!int.TryParse(Console.ReadLine(), out int age))
                age = 18;

            Console.Write("VIP-посетитель? (y/n): ");
            var isVipInput = Console.ReadLine();
            bool isVIP = isVipInput?.ToLower() == "y";

            var availableAttractions = _park.Attractions
                .Where(a => a.Status == Park2.Domain.Enums.AttractionStatus.Open && age >= a.MinAge)
                .ToList();

            if (availableAttractions.Count == 0)
            {
                Console.WriteLine("Нет подходящих аттракционов для выбранного возраста.");
                return;
            }

            Console.WriteLine("Доступные аттракционы:");
            for (int i = 0; i < availableAttractions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableAttractions[i].Name}");
            }

            Console.Write("Выберите аттракцион (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > availableAttractions.Count)
                choice = 1;

            var chosenAttraction = availableAttractions[choice - 1];

            _engine.AddManualVisitor(name, age, isVIP, chosenAttraction);

            Console.WriteLine($"Посетитель {name} успешно добавлен и направлен на аттракцион {chosenAttraction.Name}.");
        }

        private void ChangeAttractionStatus()
        {
            var selected = MenuPrinter.ChooseAttractionMenu(_park);

            Console.WriteLine("\nДоступные статусы:");
            foreach (var status in Enum.GetValues(typeof(AttractionStatus)))
            {
                Console.WriteLine($"- {status}");
            }

            Console.Write("Введите новый статус: ");
            var statusInput = Console.ReadLine();

            if (!Enum.TryParse<AttractionStatus>(statusInput, true, out var newStatus))
            {
                Console.WriteLine("❌ Неверный статус.");
                return;
            }

            int restartMinutes = 0;

            if (newStatus == AttractionStatus.Maintenance)
            {
                Console.Write("Введите время обслуживания (в минутах): ");
                if (!int.TryParse(Console.ReadLine(), out restartMinutes) || restartMinutes <= 0)
                {
                    Console.WriteLine("❌ Неверное значение времени.");
                    return;
                }
            }

            if (_engine.TryChangeAttractionStatus(selected, newStatus, restartMinutes))
                Console.WriteLine($"✅ Статус аттракциона '{selected.Name}' изменён на {newStatus}.");
            else
                Console.WriteLine("⚠️ Статус уже установлен или не удалось изменить.");
        }

        private void GetVisitorsForAttraction()
        {
            var selected = MenuPrinter.ChooseAttractionMenu(_park);

            var totalVisitors = _reportController.GetTotalVisitorsForAttraction(selected.Id);

            Console.WriteLine($"На данный момент - ({_simulatedTime:HH:mm}), общее количество посетителей посетивший данный атрацион - {totalVisitors}");
        }

        private void GetAverageWaitTimeForAttraction()
        {
            var selected = MenuPrinter.ChooseAttractionMenu(_park);

            var averageWaitTime = _reportController.GetAverageWaitTimeForAttraction(selected.Id);

            Console.WriteLine($"На данный момент - ({_simulatedTime:HH:mm}), среднее время ожидания в очереди составляет - {averageWaitTime} минут");
        }

        private void GetPeakQueueLengthForAttraction()
        {
            var selected = MenuPrinter.ChooseAttractionMenu(_park);

            var peakLength = _reportController.GetPeakQueueLengthForAttraction(selected.Id);

            Console.WriteLine($"На данный момент - ({_simulatedTime:HH:mm}), пиковое количество посетителей на данный аттракцион составляет - {peakLength} человек");
        }

        private void OnTimeTick(DateTime currentTime)
        {
            _simulatedTime = currentTime;
        }
    }
}
