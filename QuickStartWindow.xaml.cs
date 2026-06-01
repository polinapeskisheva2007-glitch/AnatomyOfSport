using System.Collections.Generic;
using System.Windows;
using AnatomyOfSport.Data;
using AnatomyOfSport.Models;

namespace AnatomyOfSport
{
    // Окно "Быстрый старт"
    public partial class QuickStartWindow : Window
    {
        private DatabaseHelper db = new();
        // Конструктор окна быстрого старта
        public QuickStartWindow()
        {
            InitializeComponent();

            MinimizeButton.Click += (s, e) => this.WindowState = WindowState.Minimized;// Кнопка сворачивания окна
            CloseButton.Click += (s, e) => this.Close();// Кнопка закрытия окна
            BackButton.Click += (s, e) => { new MainWindow().Show(); this.Close(); };// Кнопка "Назад"
            HomeButton.Click += (s, e) => { new MainWindow().Show(); this.Close(); };// Кнопка "Главная"
            LibraryButton.Click += (s, e) => { new LibraryWindow().Show(); this.Close(); };// Кнопка "Библиотека"
            HistoryButton.Click += (s, e) => { new HistoryWindow().Show(); this.Hide(); };// Кнопка "История"

            HelpButton.Click += (s, e) => App.ShowHelp(80);

            this.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.F1)
                {
                    App.ShowHelp(80);
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
                { new MyWorkoutWindow().Show(); this.Hide(); }
            };

            // Кнопки быстрого старта
            FullBodyButton.Click += (s, e) => GenerateWorkout("fullbody");
            LowerBodyButton.Click += (s, e) => GenerateWorkout("lower");
            UpperBodyButton.Click += (s, e) => GenerateWorkout("upper");
        }

        private void GenerateWorkout(string type)
        {
            // Список мышц для каждого типа тренировки
            List<string> muscles = type switch
            {
                "lower" => new List<string> {
                    "квадрицепс", "бицепс бедра", "икроножные мышцы",
                    "приводящие мышцы", "сгибатели бедра",
                    "ягодичная мышца", "большая ягодичная мышца" },
                "upper" => new List<string> {
                    "трицепс", "бицепс", "сгибатели предплечья",
                    "большая грудная мышца", "малая грудная мышца",
                    "широчайшие мышцы спины", "разгибатели спины",
                    "передняя дельта" },
                _ => new List<string>() // fullbody — все упражнения
            };

            // Получаем случайные упражнения через DatabaseHelper
            var exercises = db.GetRandomExercises(muscles, 6);

            if (exercises.Count == 0)
            {
                System.Windows.MessageBox.Show("Не удалось найти упражнения.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Конвертируем Exercise в WorkoutExercise
            var workoutExercises = new List<WorkoutExercise>();
            foreach (var ex in exercises)
            {
                workoutExercises.Add(new WorkoutExercise
                {
                    ExerciseName = ex.Name,
                    Sets = 3,
                    Reps = 12
                });
            }
            // Запускаем тренировку
            new WorkoutWindow(workoutExercises).Show();
            this.Close();
        }
    }
}