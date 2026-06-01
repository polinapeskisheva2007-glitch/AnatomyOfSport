namespace AnatomyOfSport.Models
{
    // Модель данных, представляющая группу мышц
    public class MuscleGroup
    {
        public int Id { get; set; }// Уникальный идентификатор группы мышц в базе данных
        public string Name { get; set; } = "";// Конкретная мышца 
        public string GlobalName { get; set; } = ""; // Общая группа 
    }
}
