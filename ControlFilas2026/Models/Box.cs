namespace ControlFilas2026.Models;

public class Box
{
    public Box(int id)
    {
        Id = id;
        Name = $"Caja {id}";
        IsEnabled = true;
    }

    public int Id { get; }
    public string Name { get; }
    public bool IsEnabled { get; set; }
    public string? CurrentTicket { get; set; }
}
