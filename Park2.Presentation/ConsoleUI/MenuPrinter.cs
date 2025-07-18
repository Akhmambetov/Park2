using Park2.Domain.Models;
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

        public static void ShowReportMenu()
        {
            Console.Clear();
            Console.WriteLine("Выберите пункт, который вас интересует");
            Console.WriteLine("==============================");
            Console.WriteLine("1. Общий доход от всех атракционов за симуляцию");
            Console.WriteLine("2. Общее количество посетителей, посетивших каждый аттракцион");
            Console.WriteLine("3. Среднее время ожидания в очереди для каждого аттракциона");
            Console.WriteLine("4. Пиковое количество посетителей в очереди для каждого аттракциона");
            Console.WriteLine("5. Выйти в главное меню");
            Console.Write("Выберите пункт: ");
        }

        public static Attraction? ChooseAttractionMenu(Park _park)
        {
            Console.Clear();
            Console.WriteLine("Выберите пожалуйста по которому нужна данная информация:");

            for (int i = 0; i < _park.Attractions.Count; i++)
            {
                var attraction = _park.Attractions[i];
                Console.WriteLine($"{i + 1}. {attraction.Name} (Статус: {attraction.Status})");
            }

            Console.Write("\nВведите номер аттракциона: ");

            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > _park.Attractions.Count)
            {
                Console.WriteLine("❌ Неверный ввод.");
                return null;
            }

            var selectedAttraction = _park.Attractions[index - 1];

            return selectedAttraction;
        }
    }
}
