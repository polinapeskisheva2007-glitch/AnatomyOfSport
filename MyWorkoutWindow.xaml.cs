using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AnatomyOfSport.Models;

namespace AnatomyOfSport
{
    // Окно текущей (моей) тренировки
    public partial class MyWorkoutWindow : Window
    {
        // Конструктор окна "Моя тренировка"
        public MyWorkoutWindow()
        {
            InitializeComponent();
            LoadWorkout();// Загружаем текущий список упражнений

            MinimizeButton.Click += (s, e) => this.WindowState = WindowState.Minimized;// Кнопка сворачивания окна
            CloseButton.Click += (s, e) => this.Close();// Кнопка закрытия окна
            BackButton.Click += (s, e) => { new MainWindow().Show(); this.Close(); };// Кнопка "Назад"
            HomeButton.Click += (s, e) => { new MainWindow().Show(); this.Close(); };// Кнопка "Главная"
            LibraryButton.Click += (s, e) => { new LibraryWindow().Show(); this.Close(); };// Кнопка "Библиотека"
            HistoryButton.Click += (s, e) => { new HistoryWindow().Show(); this.Hide(); };// Кнопка "История"
            HelpButton.Click += (s, e) => App.ShowHelp(85);

            this.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.F1)
                {
                    App.ShowHelp(85);
                    e.Handled = true;
                }
            };
            // Кнопка "Начать тренировку"
            StartWorkoutButton.Click += (s, e) =>
            {
                if (App.CurrentWorkout.Count == 0)
                {
                    System.Windows.MessageBox.Show("Нет упражнений для тренировки!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                new WorkoutWindow(App.CurrentWorkout).Show();
                this.Close();
            };
        }

        // Загружает список упражнений текущей тренировки
        private void LoadWorkout()
        {
            ExercisesList.ItemsSource = null;

            if (App.CurrentWorkout.Count == 0) return;

            ExercisesList.ItemsSource = App.CurrentWorkout;

            // После рендера заполняем ComboBox числами
            ExercisesList.Loaded += (s, e) => FillCombos();
        }

        // Заполняем ComboBox числами и выставляем текущие значения
        private void FillCombos()
        {
            for (int i = 0; i < App.CurrentWorkout.Count; i++)
            {
                // Получаем контейнер карточки
                if (ExercisesList.ItemContainerGenerator
                    .ContainerFromIndex(i) is not FrameworkElement container) continue;

                var setsCombo = FindChild<System.Windows.Controls.ComboBox>(container, "SetsCombo");
                var repsCombo = FindChild<System.Windows.Controls.ComboBox>(container, "RepsCombo");

                // Заполняем подходы 1-6
                if (setsCombo != null && setsCombo.Items.Count == 0)
                {
                    for (int j = 1; j <= 6; j++) setsCombo.Items.Add(j);
                    setsCombo.SelectedItem = App.CurrentWorkout[i].Sets;
                }

                // Заполняем повторения 1-20
                if (repsCombo != null && repsCombo.Items.Count == 0)
                {
                    for (int j = 1; j <= 20; j++) repsCombo.Items.Add(j);
                    repsCombo.SelectedItem = App.CurrentWorkout[i].Reps;
                }
            }
        }

        // Поиск элемента по имени внутри шаблона
        private T? FindChild<T>(DependencyObject parent, string name)
            where T : FrameworkElement
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper
                .GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is T element && element.Name == name)
                    return element;
                var result = FindChild<T>(child, name);
                if (result != null) return result;
            }
            return null;
        }

        // Удалить упражнение
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is WorkoutExercise exercise)
            {
                App.CurrentWorkout.Remove(exercise);
                LoadWorkout();
            }
        }

        // Изменение подходов
        private void SetsCombo_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.ComboBox combo &&
                combo.Tag is WorkoutExercise exercise &&
                combo.SelectedItem is int sets)
                exercise.Sets = sets;
        }

        // Изменение повторений
        private void RepsCombo_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.ComboBox combo &&
                combo.Tag is WorkoutExercise exercise &&
                combo.SelectedItem is int reps)
                exercise.Reps = reps;
        }
    } 
}