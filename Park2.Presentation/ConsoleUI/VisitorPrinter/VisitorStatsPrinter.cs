using Park2.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Presentation.ConsoleUI.VisitorPrinter
{
    public interface IVisitorStatsPrinter
    {
        void ShowVisitorStats(List<Visitor> activeVisitors, List<Visitor> departedVisitors, int maxVisitors);
    }
    public class VisitorStatsPrinter : IVisitorStatsPrinter
    {
        private readonly IOutputWriter _output;

        public VisitorStatsPrinter(IOutputWriter output)
        {
            _output = output;
        }

        public void ShowVisitorStats(List<Visitor> activeVisitors, List<Visitor> departedVisitors, int maxVisitors)
        {
            _output.WriteLine("👥 Статистика посетителей:\n");
            int totalVisitors = activeVisitors.Count + departedVisitors.Count;

            _output.WriteLine($"   В парке: {activeVisitors.Count}");
            _output.WriteLine($"   Ушли: {departedVisitors.Count}");
            _output.WriteLine($"   Всего уникальных: {totalVisitors}/{maxVisitors}\n");

            PrintVisitors("📌 Активные посетители:", activeVisitors, false);
            PrintVisitors("🚶 Ушедшие посетители:", departedVisitors, true);
        }

        private void PrintVisitors(string header, List<Visitor> visitors, bool departed)
        {
            _output.WriteLine(header);

            if (visitors.Count == 0)
            {
                _output.WriteLine("   (Нет данных)");
                return;
            }

            foreach (var visitor in visitors)
            {
                var prefix = departed ? "   🛫" : "   👤";
                _output.WriteLine($"{prefix} {visitor.Name,-15} | Возраст: {visitor.Age,2} | VIP: {visitor.IsVIP} | Прибыл: {visitor.ArrivalTime:HH:mm}");
            }

            _output.WriteLine("");
        }
    }
}
