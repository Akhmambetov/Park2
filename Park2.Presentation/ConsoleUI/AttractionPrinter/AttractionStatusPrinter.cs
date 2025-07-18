using Park2.Domain.Enums;
using Park2.Domain.Models;
using Park2.Presentation.ConsoleUI.Menu;
using System.Collections.Generic;

namespace Park2.Presentation.ConsoleUI.AttractionPrinter
{
    public interface IAttractionStatusPrinter
    {
        void PrintStatus(List<Attraction> attractions);
    }
    public class AttractionStatusPrinter : IAttractionStatusPrinter
    {
        private readonly IOutputWriter _writer;

        public AttractionStatusPrinter(IOutputWriter writer)
        {
            _writer = writer;
        }

        public void PrintStatus(List<Attraction> attractions)
        {
            _writer.Clear();
            _writer.WriteLine("🎢 Статус аттракционов:\n");

            foreach (var attraction in attractions)
            {
                _writer.WriteLine($"- {attraction.Name}");
                _writer.WriteLine($"  Статус: {attraction.Status}");
                _writer.WriteLine($"  Очередь: {attraction.CurrentQueueLength} чел.");
                _writer.WriteLine($"  Занято мест: {attraction.OccupiedSlotsCount}/{attraction.Capacity}");
                _writer.WriteLine();
            }
        }
    }
}
