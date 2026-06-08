using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AnatomyOfSport.Data;
using AnatomyOfSport.Models;

namespace AnatomyOfSport
{
    // Окно со списком упражнений для выбранной группы мышц
    public partial class ExercisesListWindow : Window
    {
        // Используется кнопкой "Назад" для восстановления предыдущего экрана
        private Window previousWindow;
        private DatabaseHelper db = new();

        // Конструктор окна списка упражнений
        public ExercisesListWindow(string muscleGroup, Window previous)
        {
            InitializeComponent();// Инициализация компонентов WPF из XAML
            previousWindow = previous;// Сохраняем ссылку на предыдущее окно
            TitleTextBlock.Text = muscleGroup;// Устанавливаем заголовок окна в название группы мышц

            // Загружаем упражнения через DatabaseHelper
            // Получаем список конкретных названий мышц для запроса
            var muscleNames = GetMuscleNames(muscleGroup);
            var exercises = db.GetExercisesByMuscleList(muscleNames);
            ExercisesList.ItemsSource = exercises;

            MinimizeButton.Click += (s, e) => this.WindowState = WindowState.Minimized;// Кнопка сворачивания окна
            CloseButton.Click += (s, e) => this.Close();// Кнопка закрытия окна
            BackButton.Click += (s, e) => { previousWindow?.Show(); this.Close(); }; // Кнопка "Назад" возврат к предыдущему окну
            HomeButton.Click += (s, e) => { new MainWindow().Show(); this.Close(); };// Кнопка "Главная"
            HistoryButton.Click += (s, e) => { new HistoryWindow().Show(); this.Close(); };// Кнопка "История"
            HelpButton.Click += (s, e) => App.ShowHelp(81);

            this.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.F1)
                {
                    App.ShowHelp(81);
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

        // Обработчик нажатия кнопки "Открыть" у элемента списка упражнений
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is string name)
                new ExerciseCardWindow(name).Show();
        }

        // Преобразует общее название группы мышц в список конкретных названий мышц для поиска в БД
        private List<string> GetMuscleNames(string groupName)
        {
            return groupName switch
            {
                "НОГИ" => new List<string> {
                    "квадрицепс", "бицепс бедра", "икроножные мышцы",
                    "приводящие мышцы", "сгибатели бедра", "ягодичная мышца" },
                "РУКИ" => new List<string> {
                    "трицепс", "сгибатели предплечья", "бицепс" },
                "СПИНА" => new List<string> {
                    "широчайшие мышцы спины", "разгибатели спины" },
                "ГРУДЬ" => new List<string> {
                    "большая грудная мышца", "малая грудная мышца",
                    "средняя грудь", "верх груди", "низ груди" },
                "ПРЕСС" => new List<string> {
                    "прямая мышца живота", "косые мышцы живота",
                    "поперечная мышца живота" },
                "ЯГОДИЦЫ" => new List<string> {
                    "ягодичная мышца", "большая ягодичная мышца",
                    "средняя ягодичная мышца", "малая ягодичная мышца" },
                _ => new List<string> { groupName }
            };
        }
    }
}