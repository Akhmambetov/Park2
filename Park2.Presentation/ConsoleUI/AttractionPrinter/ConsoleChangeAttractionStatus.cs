using Park2.Application.Simulation;
using Park2.Domain.Enums;
using Park2.Domain.Models;
using Park2.Presentation.ConsoleUI.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Presentation.ConsoleUI.AttractionPrinter
{
    public interface IConsoleChangeAttractionStatus
    {
        void ChangeAttractionStatus(Park park);
    }
    public class ConsoleChangeAttractionStatus : IConsoleChangeAttractionStatus
    {
        private readonly IMenuPrinter _printer;
        private readonly SimulationEngine _engine;

        public ConsoleChangeAttractionStatus(IMenuPrinter printer, SimulationEngine engine)
        {
            _printer = printer;
            _engine = engine;
        }
        public void ChangeAttractionStatus(Park park)
        {
            var selected = _printer.ChooseAttractionMenu(park);

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
    }
}
