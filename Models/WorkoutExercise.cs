namespace AnatomyOfSport.Models
{
    // Модель данных, представляющая упражнение в конкретной тренировки
    public class WorkoutExercise
    {
        public string ExerciseName { get; set; } = "";// Название упражнения
        public int Sets { get; set; } = 3;// Количество подходов
        public int Reps { get; set; } = 12;// Количество повторений
    }
}
