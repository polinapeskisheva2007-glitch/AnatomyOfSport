using System.Windows;

namespace AnatomyOfSport
{
    // Второе окно библиотеки упражнений
    public partial class LibrarySecondWindow : Window
    {
        // Конструктор второго окна библиотеки
        public LibrarySecondWindow()
        {
            InitializeComponent();// Инициализация компонентов WPF из XAML

            CloseButton.Click += (s, e) => this.Close();// Кнопка закрытия окна
            BackButton.Click += (s, e) => { new MainWindow().Show(); this.Close(); };// Кнопка "Назад"
            BackArrowButton.Click += (s, e) => { new LibraryWindow().Show(); this.Close(); };// Кнопка "Назад" (стрелка)  возврат к первому окну библиотеки
            ForwardArrowButton.Click += (s, e) => { new LibraryThirdWindow().Show(); this.Close(); };// Кнопка "Вперёд" (стрелка) переход к третьему окну библиотеки
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
            // Кнопка "СПИНА" — открывает список упражнений для мышц спины
            BackCollectionButton.Click += (s, e) =>
            { new ExercisesListWindow("СПИНА", this).Show(); this.Hide(); };
            // Кнопка "ГРУДЬ" — открывает список упражнений для мышц груди
            ChestCollectionButton.Click += (s, e) =>
            { new ExercisesListWindow("ГРУДЬ", this).Show(); this.Hide(); };
        }
    }
}