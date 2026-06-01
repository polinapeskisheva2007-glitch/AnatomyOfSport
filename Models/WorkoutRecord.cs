using System.Collections.Generic;

namespace AnatomyOfSport.Models
{
    // Модель данных, представляющая запись о завершенной тренировке
    public class WorkoutRecord
    {
        public string Date { get; set; } = "";// Дата и время проведения тренировки
        public int DurationSeconds { get; set; }// Общая продолжительность тренировки в секундах
        public List<WorkoutExercise> Exercises { get; set; } = new();// Список упражнений, выполненных во время тренировки

        // Форматированная строка продолжительности тренировки
        public string FormattedDuration =>
            $"{DurationSeconds / 60:00}:{DurationSeconds % 60:00}";
    }
}