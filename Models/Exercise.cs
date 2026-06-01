using System.Collections.Generic;

namespace AnatomyOfSport.Models
{
    // Модель данных, содердит информацию об упражнениях
    public class Exercise
    {
        public int Id { get; set; } // Уникальный идентификатор упражнения в базе данных
        public string Name { get; set; } = "";// Название упражнения
        public string Description { get; set; } = "";// Подробное описание техники выполнения
        public string Equipment { get; set; } = "";// Необходимое оборудование
        public List<string> MuscleGroups { get; set; } = new();// Список групп мышц
    }
}
