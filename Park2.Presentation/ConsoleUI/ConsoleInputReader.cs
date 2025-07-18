using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Presentation.ConsoleUI
{
    public interface IInputReader
    {
        string? ReadLine();
        ConsoleKeyInfo ReadKey(bool intercept = false);
    }
    public class ConsoleInputReader : IInputReader
    {
        public string? ReadLine() => Console.ReadLine();
        public ConsoleKeyInfo ReadKey(bool intercept = false) => Console.ReadKey(intercept);
    }
}
