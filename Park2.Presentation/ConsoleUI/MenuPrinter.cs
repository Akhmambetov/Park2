using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Presentation.ConsoleUI
{
    public class MenuPrinter
    {
        public static void ShowMainMenu(DateTime currentTime)
        {
            Console.Clear();
            Console.WriteLine("🎢 PARK SIMULATOR MENU 🎢");
            Console.WriteLine("==============================");
            Console.WriteLine($"1. Нынешнее время симуляции: {currentTime:HH:mm}");
            Console.WriteLine("2. Состояние аттракционов");
            Console.WriteLine("3. Посетители в парке");
            Console.WriteLine("4. Остановить поступление клиентов");
            Console.WriteLine("5. Изменить статус аттракциона");
            Console.WriteLine("6. Репорты");
            Console.WriteLine("N. Новый пользователь");
            Console.WriteLine("==============================");
            Console.Write("Выберите пункт: ");
        }
    }
}
