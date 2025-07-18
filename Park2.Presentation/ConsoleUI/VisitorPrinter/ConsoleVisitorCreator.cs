using Park2.Application.Simulation;
using Park2.Domain.Enums;
using Park2.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Presentation.ConsoleUI.VisitorPrinter
{
    public interface IVisitorCreator
    {
        void CreateVisitorFromConsoleInput(List<Attraction> attractions, SimulationEngine engine);
    }
    public class ConsoleVisitorCreator : IVisitorCreator
    {
        private readonly IInputReader _input;
        private readonly IOutputWriter _output;

        public ConsoleVisitorCreator(IInputReader input, IOutputWriter output)
        {
            _input = input;
            _output = output;
        }

        public void CreateVisitorFromConsoleInput(List<Attraction> attractions, SimulationEngine engine)
        {
            _output.WriteLine("👤 Создание нового посетителя вручную:");

            _output.WriteLine("Введите имя:");
            var name = _input.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(name)) name = "Безымянный";

            _output.WriteLine("Введите возраст:");
            int.TryParse(_input.ReadLine(), out int age);
            age = age > 0 ? age : 18;

            _output.WriteLine("VIP-посетитель? (y/n):");
            var isVIP = string.Equals(_input.ReadLine(), "y", StringComparison.OrdinalIgnoreCase);

            var available = attractions
                .Where(a => a.Status == AttractionStatus.Open && age >= a.MinAge)
                .ToList();

            if (available.Count == 0)
            {
                _output.WriteLine("❌ Нет подходящих аттракционов.");
                return;
            }

            for (int i = 0; i < available.Count; i++)
            {
                _output.WriteLine($"{i + 1}. {available[i].Name}");
            }

            _output.WriteLine("Выберите аттракцион (номер):");
            int.TryParse(_input.ReadLine(), out int choice);
            choice = Math.Clamp(choice, 1, available.Count);

            var attraction = available[choice - 1];
            engine.AddManualVisitor(name, age, isVIP, attraction);

            _output.WriteLine($"✅ Посетитель {name} направлен на {attraction.Name}.");
        }
    }
}
