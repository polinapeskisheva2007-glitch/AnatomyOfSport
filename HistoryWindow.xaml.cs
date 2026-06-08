using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AnatomyOfSport.Models;

namespace AnatomyOfSport
{
    // Окно истории тренировок
    public partial class HistoryWindow : Window
    {
        // Конструктор окна истории тренировок
        public HistoryWindow()
        {
            InitializeComponent();// Инициализация компонентов WPF из XAML

            // Загружаем историю из БД только если список пустой
            if (App.WorkoutHistory.Count == 0)
            {
                var db = new Data.DatabaseHelper();// Создаём экземпляр для работы с БД
                App.WorkoutHistory = db.LoadWorkoutHistory();// Загружаем все записи из таблицы WorkoutHistory
            }

            // Привязываем историю к ListView в XAML
            HistoryList.ItemsSource = App.WorkoutHistory
                .AsEnumerable()
                .Reverse()
                .ToList();

            MinimizeButton.Click += (s, e) => this.WindowState = WindowState.Minimized;// Кнопка сворачивания окна
            CloseButton.Click += (s, e) => this.Close();// Кнопка закрытия окна
            BackButton.Click += (s, e) => { new MainWindow().Show(); this.Close(); };// Кнопка "Назад"
            HomeButton.Click += (s, e) => { new MainWindow().Show(); this.Close(); };// Кнопка "Главная"
            LibraryButton.Click += (s, e) => { new LibraryWindow().Show(); this.Close(); }; // Кнопка "Библиотека"
            HelpButton.Click += (s, e) => App.ShowHelp(87);

            this.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.F1)
                {
                    App.ShowHelp(87);
                    e.Handled = true;
                }
            };
            // Кнопка "Моя тренировка"
            MyWorkoutButton.Click += (s, e) =>
            {
                if (App.CurrentWorkout.Count == 0)
                    System.Windows.MessageBox.Show("Вы ещё не добавили ни одного упражнения.",
                        "Тренировка пуста", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                { new MyWorkoutWindow().Show(); this.Close(); }
            };
        }
    }
}