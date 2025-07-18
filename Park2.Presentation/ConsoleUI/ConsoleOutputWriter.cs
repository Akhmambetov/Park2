using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Presentation.ConsoleUI
{
    public interface IOutputWriter
    {
        void Write(string value);
        void WriteLine(string value = "");
        void Clear();
    }
    public class ConsoleOutputWriter : IOutputWriter
    {
        public void Write(string value) => Console.Write(value);
        public void WriteLine(string value = "") => Console.WriteLine(value);
        public void Clear() => Console.Clear();
    }
}
