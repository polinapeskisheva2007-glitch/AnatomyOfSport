using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using AnatomyOfSport.Models;
using AnatomyOfSport.Data;

namespace AnatomyOfSport
{
    // Окно выполнения тренировки
    public partial class WorkoutWindow : Window
    {
        private DispatcherTimer timer;// Таймер для отсчёта времени тренировки
        private int currentSeconds = 0;// Общее количество секунд
        private bool isRunning = false;// Флаг, указывающий

        private List<WorkoutExercise> workoutExercises = new();// Список упражнений текущей тренировки
        private int currentExerciseIndex = 0;// Индекс текущего упражнения в списке
        private int currentSet = 1;// Номер текущего подхода для активного упражнения

        // Конструктор окна тренировки
        public WorkoutWindow(List<WorkoutExercise> exercises)
        {
            InitializeComponent();

            workoutExercises = exercises;
            currentExerciseIndex = 0;
            currentSet = 1;
            // Настройка таймера
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            if (workoutExercises.Count > 0)
                LoadCurrentExercise();
            else
            {
                ExerciseNameTextBlock.Text = "Нет упражнений";
                SetsRepsTextBlock.Text = "0 x 0";
                ProgressTextBlock.Text = "0 / 0 подходов";
            }

            isRunning = false;
            PauseButton.Content = "▶ СТАРТ";

            SetupButtons();

            // Загружаем видео для первого упражнения
            if (workoutExercises.Count > 0)
                LoadVideo(workoutExercises[0].ExerciseName);

            // Зацикливаем видео
            ExerciseVideo.MediaEnded += (s, e) =>
            {
                ExerciseVideo.Position = TimeSpan.Zero;
                ExerciseVideo.Play();
            };
        }
        // Настраивает обработчики событий для всех кнопок окна
        private void SetupButtons()
        {
            MinimizeButton.Click += (s, e) => this.WindowState = WindowState.Minimized;
            CloseButton.Click += (s, e) => this.Close();

            BackButton.Click += (s, e) =>
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            };

            // Кнопка помощи
            HelpButton.Click += (s, e) => App.ShowHelp(83);

            this.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.F1)
                {
                    App.ShowHelp(83);
                    e.Handled = true;
                }
            };
            // Кнопки управления тренировкой
            PauseButton.Click += (s, e) => ToggleTimer();
            ResetButton.Click += (s, e) => ResetTimer();
            CompleteSetButton.Click += (s, e) => CompleteCurrentSet();
            SkipExerciseButton.Click += (s, e) => SkipCurrentExercise();
            FinishWorkoutButton.Click += (s, e) => CompleteWorkout();
        }
        // Загружает данные текущего упражнения на экран
        private void LoadCurrentExercise()
        {
            if (currentExerciseIndex >= workoutExercises.Count) return;
            var exercise = workoutExercises[currentExerciseIndex];
            ExerciseNameTextBlock.Text = exercise.ExerciseName;
            currentSet = 1;
            UpdateSetsRepsDisplay();
            // Обновляем видео при смене упражнения
            LoadVideo(exercise.ExerciseName);
        }
        // Обновляет отображение текущего подхода и прогресса
        private void UpdateSetsRepsDisplay()
        {
            if (currentExerciseIndex >= workoutExercises.Count) return;
            var exercise = workoutExercises[currentExerciseIndex];
            SetsRepsTextBlock.Text = $"{currentSet} x {exercise.Reps}";
            ProgressTextBlock.Text = $"{currentSet} / {exercise.Sets} подходов";
        }
        // Обработчик тика таймера (вызывается каждую секунду)
        private void Timer_Tick(object? sender, EventArgs e)
        {
            currentSeconds++;
            UpdateTimerDisplay();
        }
        // Обновляет текстовое отображение таймера
        private void UpdateTimerDisplay()
        {
            int minutes = currentSeconds / 60;
            int seconds = currentSeconds % 60;
            TimerTextBlock.Text = $"{minutes:00}:{seconds:00}";
        }
        // Переключает состояние таймера
        private void ToggleTimer()
        {
            if (isRunning)
            {
                timer.Stop();
                PauseButton.Content = "▶ СТАРТ";
            }
            else
            {
                timer.Start();
                PauseButton.Content = "⏸ ПАУЗА";
            }
            isRunning = !isRunning;
        }
        // Сбрасывает таймер на ноль и останавливает его
        private void ResetTimer()
        {
            timer.Stop();
            isRunning = false;
            currentSeconds = 0;
            UpdateTimerDisplay();
            PauseButton.Content = "▶ СТАРТ";
        }
        // Завершает текущий подход
        private void CompleteCurrentSet()
        {
            if (currentExerciseIndex >= workoutExercises.Count) return;
            var exercise = workoutExercises[currentExerciseIndex];

            if (currentSet < exercise.Sets)
            {
                currentSet++;
                UpdateSetsRepsDisplay();
                System.Windows.MessageBox.Show($"Подход {currentSet - 1} завершён! Переходим к подходу {currentSet}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show($"Упражнение \"{exercise.ExerciseName}\" завершено!",
                    "Отлично", MessageBoxButton.OK, MessageBoxImage.Information);
                NextExercise();
            }
        }
        // Пропускает текущее упражнение и переходит к следующему
        private void SkipCurrentExercise()
        {
            if (currentExerciseIndex >= workoutExercises.Count) return;
            var exercise = workoutExercises[currentExerciseIndex];
            System.Windows.MessageBox.Show($"Упражнение \"{exercise.ExerciseName}\" пропущено",
                "Пропуск", MessageBoxButton.OK, MessageBoxImage.Information);
            NextExercise();
        }
        // Переходит к следующему упражнению
        private void NextExercise()
        {
            currentExerciseIndex++;
            if (currentExerciseIndex < workoutExercises.Count)
                LoadCurrentExercise();
            else
                CompleteWorkout();
        }
        // Завершает тренировку
        private void CompleteWorkout()
        {
            timer.Stop();

            // Сохраняем в память
            var record = new WorkoutRecord
            {
                Date = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                DurationSeconds = currentSeconds,
                Exercises = new List<WorkoutExercise>(workoutExercises)
            };
            App.WorkoutHistory.Add(record);

            // Сохраняем в БД 
            var db = new Data.DatabaseHelper();
            db.SaveWorkoutRecord(record);

            // Очищаем текущую тренировку
            App.CurrentWorkout.Clear();

            System.Windows.MessageBox.Show(
                $"Тренировка завершена! Отличная работа!\n\nВремя тренировки: {record.FormattedDuration}",
                "Поздравляем", MessageBoxButton.OK, MessageBoxImage.Information);

            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
        // Загружает видео для упражнения из папки Videos
        private void LoadVideo(string exerciseName)
        {
            string videosFolder = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "Videos");

            if (!System.IO.Directory.Exists(videosFolder))
            {
                ExerciseVideo.Visibility = Visibility.Collapsed;
                VideoPlaceholder.Visibility = Visibility.Visible;
                return;
            }

            string[] extensions = { ".mp4", ".gif", ".avi", ".wmv" };
            string videoPath = null;

            // Ищем файл с точным именем упражнения
            foreach (var ext in extensions)
            {
                string path = System.IO.Path.Combine(videosFolder, exerciseName + ext);
                if (System.IO.File.Exists(path))
                {
                    videoPath = path;
                    break;
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
                ExerciseVideo.Visibility = Visibility.Collapsed;
                VideoPlaceholder.Visibility = Visibility.Visible;
            }
        }
    }
}