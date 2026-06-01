using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using AnatomyOfSport.Models;

namespace AnatomyOfSport
{
    // Окно с информацией о выбранном упражнении
    public partial class ExerciseCardWindow : Window
    {
        private string currentExercise; // Название текущего отображаемого упражнения

        // Конструктор окна карточки упражнения
        public ExerciseCardWindow(string exerciseName)
        {
            InitializeComponent(); // Инициализация компонентов WPF из XAML
            currentExercise = exerciseName; // Сохраняем название упражнения
            LoadExerciseData(exerciseName); // Загружаем данные из БД
            LoadVideo(exerciseName); // Загружаем видео для упражнения

            MinimizeButton.Click += (s, e) => this.WindowState = WindowState.Minimized; // Кнопка сворачивания окна
            CloseButton.Click += (s, e) => { ExerciseVideo.Stop(); this.Close(); }; // Кнопка закрытия окна
            BackButton.Click += (s, e) => { ExerciseVideo.Stop(); this.Close(); }; // Кнопка "Назад"

            HelpButton.Click += (s, e) => App.ShowHelp(82);

            this.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.F1)
                {
                    App.ShowHelp(82);
                    e.Handled = true;
                }
            };

            HomeButton.Click += (s, e) => { ExerciseVideo.Stop(); new MainWindow().Show(); this.Close(); }; // Кнопка "Главная"
            LibraryButton.Click += (s, e) => { ExerciseVideo.Stop(); new LibraryWindow().Show(); this.Close(); }; // Кнопка "Библиотека"
            HistoryButton.Click += (s, e) => { ExerciseVideo.Stop(); new HistoryWindow().Show(); this.Hide(); }; // Кнопка "История"

            // Кнопка "Моя тренировка"
            MyWorkoutButton.Click += (s, e) =>
            {
                // Проверяем, есть ли упражнения в текущей тренировке
                if (App.CurrentWorkout.Count == 0)
                    System.Windows.MessageBox.Show("Вы ещё не добавили ни одного упражнения.",
                        "Тренировка пуста", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                { new MyWorkoutWindow().Show(); this.Hide(); }
            };

            // Кнопка "Добавить в тренировку"
            AddToWorkoutButton.Click += (s, e) =>
            {
                // Проверяем, нет ли упражнения уже в тренировке
                if (!App.IsExerciseInWorkout(currentExercise))
                {
                    App.AddToWorkout(currentExercise);
                    System.Windows.MessageBox.Show($"Упражнение \"{currentExercise}\" добавлено в тренировку!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                    System.Windows.MessageBox.Show($"Упражнение \"{currentExercise}\" уже есть в тренировке!",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            };

            // Кнопка "Начать тренировку"
            StartWorkoutButton.Click += (s, e) =>
            {
                // Проверяем, есть ли это упражнение уже в текущей тренировке
                var existing = App.CurrentWorkout.Find(x => x.ExerciseName == currentExercise);
                var workoutData = new List<WorkoutExercise>();

                if (existing != null)
                    workoutData.Add(existing);
                else
                    workoutData.Add(new WorkoutExercise
                    {
                        ExerciseName = currentExercise,
                        Sets = 3,
                        Reps = 12
                    });

                ExerciseVideo.Stop(); // Останавливаем видео перед переходом
                new WorkoutWindow(workoutData).Show();
                this.Close();
            };

            // Зацикливаем видео — когда заканчивается, начинается снова
            ExerciseVideo.MediaEnded += (s, e) =>
            {
                ExerciseVideo.Position = TimeSpan.Zero;
                ExerciseVideo.Play();
            };
        }

        // Загружает видео для упражнения из папки Videos
        private void LoadVideo(string exerciseName)
        {
            string videosFolder = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "Videos");

            // Если папки нет — показываем заглушку
            if (!System.IO.Directory.Exists(videosFolder)) { ShowVideoPlaceholder(); return; }

            string[] extensions = { ".mp4", ".gif", ".avi", ".wmv" };
            string? videoPath = null;

            // Ищем файл с точным именем упражнения
            foreach (var ext in extensions)
            {
                string path = System.IO.Path.Combine(videosFolder, exerciseName + ext);
                if (System.IO.File.Exists(path)) { videoPath = path; break; }
            }

            // Если не нашли точного совпадения — ищем по частичному совпадению
            if (videoPath == null)
            {
                foreach (var ext in extensions)
                {
                    var files = System.IO.Directory.GetFiles(videosFolder, "*" + ext);
                    foreach (var file in files)
                    {
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(file).ToLower();
                        string exName = exerciseName.ToLower();
                        if (exName.Contains(fileName) || fileName.Contains(exName))
                        {
                            videoPath = file;
                            break;
                        }
                    }
                    if (videoPath != null) break;
                }
            }

            if (videoPath != null)
            {
                // Видео найдено — показываем и запускаем
                ExerciseVideo.Source = new Uri(videoPath);
                ExerciseVideo.Visibility = Visibility.Visible;
                VideoPlaceholder.Visibility = Visibility.Collapsed;
                ExerciseVideo.Play();
            }
            else
            {
                // Видео не найдено — показываем заглушку
                ShowVideoPlaceholder();
            }
        }

        // Показывает заглушку вместо видео
        private void ShowVideoPlaceholder()
        {
            ExerciseVideo.Visibility = Visibility.Collapsed;
            VideoPlaceholder.Visibility = Visibility.Visible;
        }

        // Загружает данные об упражнении из базы данных
        private void LoadExerciseData(string exerciseName)
        {
            string connectionString = "Data Source=anatomy.db;Version=3;";
            try
            {
                using var conn = new SQLiteConnection(connectionString);
                conn.Open();
                using var cmd = new SQLiteCommand(
                    "SELECT name, description, equipment FROM Exercises WHERE name = @name", conn);
                cmd.Parameters.AddWithValue("@name", exerciseName);
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ExerciseNameTextBlock.Text = reader.GetString(0);
                    DescriptionTextBlock.Text = reader.IsDBNull(1) ? "Описание не найдено" : reader.GetString(1);
                    string eq = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    EquipmentTextBlock.Text = string.IsNullOrWhiteSpace(eq) || eq == "NULL"
                        ? "Специальное оборудование не требуется" : eq;
                }
                else
                {
                    ExerciseNameTextBlock.Text = exerciseName;
                    DescriptionTextBlock.Text = "Описание не найдено";
                    EquipmentTextBlock.Text = "Специальное оборудование не требуется";
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
                ExerciseNameTextBlock.Text = exerciseName;
                DescriptionTextBlock.Text = "Ошибка загрузки описания";
                EquipmentTextBlock.Text = "Специальное оборудование не требуется";
            }
        }
    }
}