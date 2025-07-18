using Park2.Application.Interfaces;
using Park2.Application.Simulation;
using Park2.Domain.Models;
using Park2.Presentation.ConsoleUI.Menu;
using System;
using System.Collections.Generic;

namespace Park2.Presentation.ConsoleUI.Report
{
    public interface IReportMenuPrinter
    {
        void ShowReportMenu();
    }

    public class ReportMenuPrinter : IReportMenuPrinter
    {
        private readonly Park _park;
        private readonly IClockSimulationService _clock;
        private readonly IReportPrinter _printer;
        private readonly IInputReader _reader;
        private readonly IOutputWriter _writer;
        private readonly Dictionary<string, Action> _reportActions;
        private readonly IMenuPrinter _menuPrinter;

        public ReportMenuPrinter(
            Park park,
            IClockSimulationService clock,
            IReportPrinter printer,
            IInputReader reader,
            IOutputWriter writer,
            IMenuPrinter menuPrinter)
        {
            _park = park ?? throw new ArgumentNullException(nameof(park));
            _clock = clock?? throw new ArgumentNullException(nameof(clock));
            _printer = printer ?? throw new ArgumentNullException(nameof(printer));
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _menuPrinter = menuPrinter ?? throw new ArgumentNullException(nameof(menuPrinter));

            _reportActions = new Dictionary<string, Action>
            {
                ["1"] = () => _printer.PrintTotalRevenue(_clock.CurrentTime),
                ["2"] = () => _printer.PrintVisitorsForAttraction(_clock.CurrentTime, SelectAttraction()),
                ["3"] = () => _printer.PrintAverageWaitTimeForAttraction(_clock.CurrentTime, SelectAttraction()),
                ["4"] = () => _printer.PrintPeakQueueLengthForAttraction(_clock.CurrentTime, SelectAttraction())
            };
        }

        public void ShowReportMenu()
        {
            while (true)
            {
                _menuPrinter.ShowReportMenu();
                _writer.WriteLine("Выберите пункт: ");
                var input = _reader.ReadLine();

                if (input == "5")
                    return;

                if (input != null && _reportActions.TryGetValue(input, out var action))
                {
                    action();
                }
                else
                {
                    _writer.WriteLine("❌ Неверный пункт.");
                }

                _writer.Write("\nНажмите любую клавишу для возврата к меню отчётов...");
                Console.ReadKey(true); // Можно заменить тоже, если хочешь полностью отказаться от Console.*
            }
        }

        private Attraction SelectAttraction()
        {
            return _menuPrinter.ChooseAttractionMenu(_park);
        }
    }
}
