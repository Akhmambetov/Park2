using Park2.Domain.Models;
using System;

namespace Park2.Presentation.ConsoleUI.Menu
{
    public interface IMenuPrinter
    {
        void ShowMainMenu(DateTime currentTime);
        void ShowReportMenu();
        Attraction? ChooseAttractionMenu(Park park);
        void ShowWelcome();
        void ShowTime(DateTime simulatedTime);
    }
    public class MenuPrinter : IMenuPrinter
    {
        private readonly IOutputWriter _writer;
        private readonly IInputReader _reader;

        public MenuPrinter(IOutputWriter writer, IInputReader reader)
        {
            _writer = writer;
            _reader = reader;
        }

        public void ShowMainMenu(DateTime currentTime)
        {
            _writer.Clear();
            _writer.WriteLine("🎢 PARK SIMULATOR MENU 🎢");
            _writer.WriteLine("==============================");
            _writer.WriteLine($"1. Нынешнее время симуляции: {currentTime:HH:mm}");
            _writer.WriteLine("2. Состояние аттракционов");
            _writer.WriteLine("3. Посетители в парке");
            _writer.WriteLine("4. Остановить поступление клиентов");
            _writer.WriteLine("5. Изменить статус аттракциона");
            _writer.WriteLine("6. Репорты");
            _writer.WriteLine("N. Новый пользователь");
            _writer.WriteLine("==============================");
            _writer.Write("Выберите пункт: ");
        }

        public void ShowReportMenu()
        {
            _writer.Clear();
            _writer.WriteLine("Выберите пункт, который вас интересует");
            _writer.WriteLine("==============================");
            _writer.WriteLine("1. Общий доход от всех атракционов за симуляцию");
            _writer.WriteLine("2. Общее количество посетителей, посетивших каждый аттракцион");
            _writer.WriteLine("3. Среднее время ожидания в очереди для каждого аттракциона");
            _writer.WriteLine("4. Пиковое количество посетителей в очереди для каждого аттракциона");
            _writer.WriteLine("5. Выйти в главное меню");
            _writer.Write("Выберите пункт: ");
        }

        public Attraction? ChooseAttractionMenu(Park park)
        {
            while (true)
            {
                _writer.Clear();
                _writer.WriteLine("Выберите аттракцион:");

                for (int i = 0; i < park.Attractions.Count; i++)
                {
                    var a = park.Attractions[i];
                    _writer.WriteLine($"{i + 1}. {a.Name} (Статус: {a.Status})");
                }

                _writer.Write("\nВведите номер аттракциона или 0 для выхода: ");
                var input = _reader.ReadLine();

                if (int.TryParse(input, out int index))
                {
                    if (index == 0)
                        return null;

                    if (index >= 1 && index <= park.Attractions.Count)
                        return park.Attractions[index - 1];
                }

                _writer.WriteLine("❌ Неверный ввод. Попробуйте снова...");
                _writer.WriteLine("Нажмите любую клавишу...");
                _reader.ReadKey(true);
            }
        }

        public void ShowWelcome()
        {
            _writer.Clear();
            _writer.WriteLine("🎡 Добро пожаловать в симулятор парка аттракционов!");
            _writer.WriteLine();
            _writer.WriteLine("1. ▶️ Запустить симуляцию");
            _writer.WriteLine();
            _writer.Write("Введите номер пункта и нажмите Enter: ");
        }

        public void ShowTime(DateTime simulatedTime)
        {
            _writer.Clear();
            _writer.WriteLine("🕒 Текущее симулированное время:");
            _writer.WriteLine($"   {simulatedTime:HH:mm}");
        }
    }
}
