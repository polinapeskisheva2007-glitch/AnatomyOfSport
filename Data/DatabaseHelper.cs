using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using AnatomyOfSport.Models;

namespace AnatomyOfSport.Data
{
    // Класс для работы с базой данных SQLite
    public class DatabaseHelper
    {
        // Строка подключения к БД
        private readonly string ConnectionString;

        // Конструктор по умолчанию — использует основную базу anatomy.db
        public DatabaseHelper()
        {
            ConnectionString = "Data Source=anatomy.db;Version=3;";
        }
        // Конструктор для тестов — позволяет указать свой путь 
        public DatabaseHelper(string dbPath)
        {
            ConnectionString = $"Data Source={dbPath};Version=3;";
        }

        // Проверка существования БД
        public bool DatabaseExists() => File.Exists("anatomy.db");

        // Вспомогательный метод для преобразования строки из БД в объект Exercise
        private Exercise MapExercise(SQLiteDataReader reader)
        {
            return new Exercise
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Equipment = reader.IsDBNull(3) ? "" : reader.GetString(3)
            };
        }

        // Получает все упражнения из базы данных
        public List<Exercise> GetAllExercises()
        {
            var exercises = new List<Exercise>();
            // SQL-запрос для выборки всех упражнений
            string query = "SELECT id, name, description, equipment FROM Exercises";

            try
            {
                // using обеспечивает автоматическое закрытие соединения
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();// Открываем соединение с БД
                using var cmd = new SQLiteCommand(query, conn);
                using var reader = cmd.ExecuteReader(); // Выполняем запрос
                while (reader.Read())
                    exercises.Add(MapExercise(reader));// Преобразуем каждую строку в объект
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка БД: {ex.Message}");
            }
            return exercises;
        }

        // Получает упражнения, связанные с определенной группой мышц
        public List<Exercise> GetExercisesByMuscleGroup(string muscleGroupName)
        {
            var exercises = new List<Exercise>();
            string query = @"
                SELECT DISTINCT e.id, e.name, e.description, e.equipment 
                FROM Exercises e
                JOIN ExerciseMuscles em ON e.id = em.ExerciseId
                JOIN MuscleGroups mg ON em.MuscleGroupId = mg.id
                WHERE mg.nameMuscle = @muscleGroup OR mg.globalMuscle = @muscleGroup";

            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@muscleGroup", muscleGroupName);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    exercises.Add(MapExercise(reader));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка БД: {ex.Message}");
            }
            return exercises;
        }

        // Получает упражнения по фильтру группы мышц
        public List<Exercise> GetExercisesByMuscleList(List<string> muscleNames)
        {
            var exercises = new List<Exercise>();
            // Если список пустой то возвращаем пустой результат
            if (muscleNames == null || muscleNames.Count == 0) return exercises;

            // Создаем параметры для каждой группы мышц
            var paramNames = new List<string>();
            for (int i = 0; i < muscleNames.Count; i++)
                paramNames.Add($"@m{i}");

            // Формируем запрос
            string query = $@"
                SELECT DISTINCT e.id, e.name, e.description, e.equipment
                FROM Exercises e
                JOIN ExerciseMuscles em ON e.id = em.ExerciseId
                JOIN MuscleGroups mg ON em.MuscleGroupId = mg.id
                WHERE mg.nameMuscle IN ({string.Join(",", paramNames)})";

            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                using var cmd = new SQLiteCommand(query, conn);
                // Привязываем значения к параметрам
                for (int i = 0; i < muscleNames.Count; i++)
                    cmd.Parameters.AddWithValue(paramNames[i], muscleNames[i]);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    exercises.Add(MapExercise(reader));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка БД: {ex.Message}");
            }
            return exercises;
        }

        // Получает упражнение по имени
        public Exercise? GetExerciseByName(string name)
        {
            string query = "SELECT id, name, description, equipment FROM Exercises WHERE name = @name";

            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                using var reader = cmd.ExecuteReader();
                if (reader.Read()) return MapExercise(reader);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка БД: {ex.Message}");
            }
            return null;
        }

        // Получает упражнение по ID
        public Exercise? GetExerciseById(int id)
        {
            string query = "SELECT id, name, description, equipment FROM Exercises WHERE id = @id";

            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read()) return MapExercise(reader);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка БД: {ex.Message}");
            }
            return null;
        }

        // Получает все названия групп мышц
        public List<string> GetAllMuscleGroups()
        {
            var groups = new List<string>();
            string query = "SELECT DISTINCT nameMuscle FROM MuscleGroups ORDER BY nameMuscle";

            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                using var cmd = new SQLiteCommand(query, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    groups.Add(reader.GetString(0));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка БД: {ex.Message}");
            }
            return groups;
        }

        // Получает случайные упражнения для быстрого старта
        public List<Exercise> GetRandomExercises(List<string> muscleNames, int count = 6)
        {
            var exercises = new List<Exercise>();

            string muscleFilter = "";
            var paramNames = new List<string>();

            if (muscleNames != null && muscleNames.Count > 0)
            {
                for (int i = 0; i < muscleNames.Count; i++)
                    paramNames.Add($"@m{i}");
                muscleFilter = $"WHERE mg.nameMuscle IN ({string.Join(",", paramNames)})";
            }

            string query = $@"
                SELECT DISTINCT e.id, e.name, e.description, e.equipment
                FROM Exercises e
                JOIN ExerciseMuscles em ON e.id = em.ExerciseId
                JOIN MuscleGroups mg ON em.MuscleGroupId = mg.id
                {muscleFilter}
                ORDER BY RANDOM()
                LIMIT @count";

            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                using var cmd = new SQLiteCommand(query, conn);
                for (int i = 0; i < paramNames.Count; i++)
                    cmd.Parameters.AddWithValue(paramNames[i], muscleNames[i]);
                cmd.Parameters.AddWithValue("@count", count);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    exercises.Add(MapExercise(reader));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка БД: {ex.Message}");
            }
            return exercises;
        }

        // Сохраняет запись о завершенной тренировке в историю
        public void SaveWorkoutRecord(WorkoutRecord record)
        {
            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();

                // Создаём таблицу если не существует
                string createTable = @"
                    CREATE TABLE IF NOT EXISTS WorkoutHistory (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT NOT NULL,
                        DurationSeconds INTEGER NOT NULL,
                        Exercises TEXT NOT NULL
                    )";
                using var createCmd = new SQLiteCommand(createTable, conn);
                createCmd.ExecuteNonQuery();

                // Сохраняем упражнения как текст
                var exerciseLines = new List<string>();
                foreach (var ex in record.Exercises)
                    exerciseLines.Add($"{ex.ExerciseName}|{ex.Sets}|{ex.Reps}");
                string exercisesText = string.Join(";", exerciseLines);

                string insert = @"
                    INSERT INTO WorkoutHistory (Date, DurationSeconds, Exercises)
                    VALUES (@date, @duration, @exercises)";
                using var insertCmd = new SQLiteCommand(insert, conn);
                insertCmd.Parameters.AddWithValue("@date", record.Date);
                insertCmd.Parameters.AddWithValue("@duration", record.DurationSeconds);
                insertCmd.Parameters.AddWithValue("@exercises", exercisesText);
                insertCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения истории: {ex.Message}");
            }
        }

        // Загружает историю тренировок из БД
        public List<WorkoutRecord> LoadWorkoutHistory()
        {
            var history = new List<WorkoutRecord>();

            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();

                // Если таблицы нет — возвращаем пустой список
                string checkTable = "SELECT name FROM sqlite_master WHERE type='table' AND name='WorkoutHistory'";
                using var checkCmd = new SQLiteCommand(checkTable, conn);
                if (checkCmd.ExecuteScalar() == null) return history;

                string query = "SELECT Date, DurationSeconds, Exercises FROM WorkoutHistory ORDER BY Id DESC";
                using var cmd = new SQLiteCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var record = new WorkoutRecord
                    {
                        Date = reader.GetString(0),
                        DurationSeconds = reader.GetInt32(1),
                        Exercises = new List<WorkoutExercise>()
                    };

                    // Разбираем строку упражнений обратно в объекты
                    string exercisesText = reader.GetString(2);
                    foreach (var line in exercisesText.Split(';'))
                    {
                        var parts = line.Split('|');
                        if (parts.Length == 3)
                        {
                            record.Exercises.Add(new WorkoutExercise
                            {
                                ExerciseName = parts[0],
                                Sets = int.Parse(parts[1]),
                                Reps = int.Parse(parts[2])
                            });
                        }
                    }
                    history.Add(record);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки истории: {ex.Message}");
            }
            return history;
        }
        // Получает все виды оборудования
        public List<string> GetAllEquipment()
        {
            var equipment = new List<string>();
            string query = @"SELECT DISTINCT equipment FROM Exercises 
                     WHERE equipment IS NOT NULL 
                     AND equipment != '' 
                     AND equipment != 'NULL' 
                     ORDER BY equipment";
            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                using var cmd = new SQLiteCommand(query, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string eq = reader.GetString(0);
                    if (!string.IsNullOrWhiteSpace(eq) && eq != "NULL")
                        equipment.Add(eq);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка БД: {ex.Message}");
            }
            return equipment;
        }

        // Получает все упражнения с группами мышц
        public List<Exercise> GetAllExercisesWithMuscles()
        {
            var exercises = new List<Exercise>();
            string query = @"
                    SELECT e.id, e.name, e.description, e.equipment,
                    GROUP_CONCAT(mg.nameMuscle, ',') as muscles
                    FROM Exercises e
                    LEFT JOIN ExerciseMuscles em ON e.id = em.ExerciseId
                    LEFT JOIN MuscleGroups mg ON em.MuscleGroupId = mg.id
                    GROUP BY e.id";
            try
            {
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                using var cmd = new SQLiteCommand(query, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var exercise = new Exercise
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        Equipment = reader.IsDBNull(3) ? "" : reader.GetString(3)
                    };
                    if (!reader.IsDBNull(4))
                        exercise.MuscleGroups = reader.GetString(4)
                            .Split(',')
                            .Select(m => m.Trim())
                            .ToList();
                    exercises.Add(exercise);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка БД: {ex.Message}");
            }
            return exercises;
        }
    }
}