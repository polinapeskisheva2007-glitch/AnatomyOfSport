using System.Windows;
using System.Windows.Input;

namespace AnatomyOfSport
{
    // Главное окно приложения
    public partial class MainWindow : Window
    {
        // Конструктор главного окна приложения
        public MainWindow()
        {
            InitializeComponent();// Инициализация компонентов WPF из XAML

            MinimizeButton.Click += (s, e) => this.WindowState = WindowState.Minimized;// Кнопка сворачивания окна
            CloseButton.Click += (s, e) => this.Close();// Кнопка закрытия окна

            HelpButton.Click += (s, e) => App.ShowHelp(78);

            this.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.F1)
                {
                    App.ShowHelp(78);
                    e.Handled = true;
                }
            };

            LibraryButton.Click += (s, e) => { new LibraryWindow().Show(); this.Hide(); };// Кнопка "Библиотека"
            HistoryButton.Click += (s, e) => { new HistoryWindow().Show(); this.Hide(); };// Кнопка "История"
            QuickStartButton.Click += (s, e) => { new QuickStartWindow().Show(); this.Hide(); };// Кнопка "Быстрый старт"
            SearchButton.Click += (s, e) => { new SearchWindow().Show(); this.Hide(); };// Кнопка "Поиск"

            // Кнопка "Моя тренировка"
            MyWorkoutButton.Click += (s, e) =>
            {
                if (App.CurrentWorkout.Count == 0)
                    System.Windows.MessageBox.Show(
                        "Вы ещё не добавили ни одного упражнения.\nОткройте Библиотеку → выберите упражнение → нажмите 'ДОБАВИТЬ В ТРЕНИРОВКУ'",
                        "Тренировка пуста", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                { new MyWorkoutWindow().Show(); this.Hide(); }
            };

            // Кнопки групп мышц
            ChestButton.Click += (s, e) =>
            { new ExercisesListWindow("ГРУДЬ", this).Show(); this.Hide(); };
            BackButton.Click += (s, e) =>
            { new ExercisesListWindow("СПИНА", this).Show(); this.Hide(); };
            LegsButton.Click += (s, e) =>
            { new ExercisesListWindow("НОГИ", this).Show(); this.Hide(); };
            ShouldersButton.Click += (s, e) =>
            { new ExercisesListWindow("РУКИ", this).Show(); this.Hide(); };
            GlutesButton.Click += (s, e) =>
            { new ExercisesListWindow("ЯГОДИЦЫ", this).Show(); this.Hide(); };
            AbsButton.Click += (s, e) =>
            { new ExercisesListWindow("ПРЕСС", this).Show(); this.Hide(); };
        }
    }
}