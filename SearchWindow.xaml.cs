using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AnatomyOfSport.Data;
using AnatomyOfSport.Models;

namespace AnatomyOfSport
{
    // Окно поиска упражнений
    public partial class SearchWindow : Window
    {
        private List<Exercise> allExercises = new();
        private DatabaseHelper db = new();
        // Конструктор окна поиска
        public SearchWindow()
        {
            InitializeComponent();
            // Загружаем фильтры и все упражнения при открытии окна
            LoadFilters();
            LoadAllExercises();
            // После загрузки окна сразу применяем фильтры
            this.Loaded += (s, e) => ApplyFilters();

            MinimizeButton.Click += (s, e) => this.WindowState = WindowState.Minimized;// Кнопка сворачивания окна
            CloseButton.Click += (s, e) => this.Close();// Кнопка закрытия окна
            BackButton.Click += (s, e) => { new MainWindow().Show(); this.Close(); };// Кнопка "Назад"
            HomeButton.Click += (s, e) => { new MainWindow().Show(); this.Close(); };// Кнопка "Главная"
            LibraryButton.Click += (s, e) => { new LibraryWindow().Show(); this.Close(); };// Кнопка "Библиотека"
            HistoryButton.Click += (s, e) => { new HistoryWindow().Show(); this.Hide(); };// Кнопка "История"

            HelpButton.Click += (s, e) => App.ShowHelp(79);

            this.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.F1)
                {
                    App.ShowHelp(79);
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
        }
        // Загружает доступные фильтры
        private void LoadFilters()
        {
            // Группы мышц через DatabaseHelper
            var muscleGroups = db.GetAllMuscleGroups();
            foreach (var group in muscleGroups)
                MuscleGroupComboBox.Items.Add(new ComboBoxItem { Content = group });

            // Оборудование через DatabaseHelper
            var equipment = db.GetAllEquipment();
            foreach (var eq in equipment)
                EquipmentComboBox.Items.Add(new ComboBoxItem { Content = eq });

            EquipmentComboBox.Items.Add(new ComboBoxItem { Content = "Не требуется" });
        }
        // Загружает все упражнения из базы данных вместе с группами мышц
        private void LoadAllExercises()
        {
            allExercises = db.GetAllExercisesWithMuscles();
        }
        // Применяет все активные фильтры
        private void ApplyFilters()
        {
            // Защита от вызова до полной инициализации элементов
            if (MuscleGroupComboBox == null || EquipmentComboBox == null ||
                SearchBox == null || ResultsListBox == null) return;
            // Получаем выбранную группу мышц 
            string? selectedMuscle = null;
            if (MuscleGroupComboBox.SelectedItem is ComboBoxItem item &&
                item.Content.ToString() != "Все группы")
                selectedMuscle = item.Content.ToString();
            // Получаем выбранное оборудование
            string? selectedEquipment = null;
            if (EquipmentComboBox.SelectedItem is ComboBoxItem eqItem &&
                eqItem.Content.ToString() != "Всё оборудование")
                selectedEquipment = eqItem.Content.ToString();
            // Получаем текст поиска
            string searchText = SearchBox.Text?.ToLower() ?? "";
            // Фильтрация упражнений
            var filtered = allExercises.Where(e =>
            {
                // Поиск по названию упражнения
                bool matchSearch = string.IsNullOrEmpty(searchText) ||
                    e.Name.ToLower().Contains(searchText);
                // Фильтр по группе мышц
                bool matchMuscles = string.IsNullOrEmpty(selectedMuscle) ||
                    e.MuscleGroups.Contains(selectedMuscle);
                // Фильтр по оборудованию
                bool matchEquipment;
                if (selectedEquipment == "Не требуется")
                    matchEquipment = string.IsNullOrEmpty(e.Equipment) ||
                        e.Equipment == "Не требуется";
                else
                    matchEquipment = string.IsNullOrEmpty(selectedEquipment) ||
                        e.Equipment == selectedEquipment;

                return matchSearch && matchMuscles && matchEquipment;
            }).ToList();
            // Обновление списка результатов
            ResultsListBox.Items.Clear();
            foreach (var exercise in filtered)
                ResultsListBox.Items.Add(exercise.Name);

            if (filtered.Count == 0 && allExercises.Count > 0)
                ResultsListBox.Items.Add("Ничего не найдено");
        }
        // Обработчики событий фильтров 
        private void MuscleGroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ApplyFilters();

        private void EquipmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ApplyFilters();

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilters();
        // Обработчик выбора упражнения из списка результатов
        private void ResultsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultsListBox.SelectedItem != null &&
                ResultsListBox.SelectedItem.ToString() != "Ничего не найдено")
            {
                string selectedExercise = ResultsListBox.SelectedItem.ToString()!;
                new ExerciseCardWindow(selectedExercise).Show();
                ResultsListBox.SelectedItem = null;
            }
        }
    }
}