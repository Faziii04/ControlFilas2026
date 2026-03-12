namespace ControlFilas2026.Models
{
    public class Box
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsAvailable { get; set; }
        public int CurrentClient { get; set; }

        public Box(int id)
        {
            Id = id;
            Name = $"Caja {id}";
            IsEnabled = true;
            IsAvailable = true;
            CurrentClient = 0;
        }
    }
}
