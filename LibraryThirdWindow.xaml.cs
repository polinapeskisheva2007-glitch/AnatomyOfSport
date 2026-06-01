using System.Windows;

namespace AnatomyOfSport
{
    // Третье окно библиотеки упражнений
    public partial class LibraryThirdWindow : Window
    {
        // Конструктор третьего окна библиотеки
        public LibraryThirdWindow()
        {
            InitializeComponent();// Инициализация компонентов WPF из XAML

            CloseButton.Click += (s, e) => this.Close();// Кнопка закрытия окна
            BackButton.Click += (s, e) => { new MainWindow().Show(); this.Close(); };// Кнопка "Назад"
            BackArrowButton.Click += (s, e) => { new LibrarySecondWindow().Show(); this.Close(); };// Кнопка "Назад" (стрелка) возврат ко второму окну библиотеки
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
            // Кнопка "ПРЕСС" — открывает список упражнений для мышц живота
            AbsCollectionButton.Click += (s, e) =>
            { new ExercisesListWindow("ПРЕСС", this).Show(); this.Hide(); };
            // Кнопка "ЯГОДИЦЫ" — открывает список упражнений для ягодичных мышц
            GlutesCollectionButton.Click += (s, e) =>
            { new ExercisesListWindow("ЯГОДИЦЫ", this).Show(); this.Hide(); };
        }
    }
}