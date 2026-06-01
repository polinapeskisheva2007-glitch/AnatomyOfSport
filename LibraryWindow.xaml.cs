using System.Windows;

namespace AnatomyOfSport
{
    // Первое окно библиотеки упражнений
    public partial class LibraryWindow : Window
    {
        // Конструктор первого окна библиотеки
        public LibraryWindow()
        {
            InitializeComponent();// Инициализация компонентов WPF из XAML

            CloseButton.Click += (s, e) => this.Close();// Кнопка закрытия окна
            BackButton.Click += (s, e) => { new MainWindow().Show(); this.Close(); };// Кнопка "Назад" переход на главный экран
            ForwardButton.Click += (s, e) => { new LibrarySecondWindow().Show(); this.Close(); };// Кнопка "Вперёд" переход ко второму окну библиотеки
            HomeButton.Click += (s, e) => { new MainWindow().Show(); this.Close(); };// Кнопка "Главная"
            HistoryButton.Click += (s, e) => { new HistoryWindow().Show(); this.Hide(); };// Кнопка "История"

            HelpButton.Click += (s, e) => App.ShowHelp(86);

            this.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.F1)
                {
                    App.ShowHelp(86);
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
            // Кнопка "НОГИ" — открывает список упражнений для мышц ног
            LegsCollectionButton.Click += (s, e) =>
            { new ExercisesListWindow("НОГИ", this).Show(); this.Hide(); };
            // Кнопка "РУКИ" — открывает список упражнений для мышц рук
            ArmsCollectionButton.Click += (s, e) =>
            { new ExercisesListWindow("РУКИ", this).Show(); this.Hide(); };
        }
    }
}