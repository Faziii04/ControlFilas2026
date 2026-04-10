namespace ControlFilas2026.Models;

public class Box
{
    public Box(int id)
    {
        Id = id;
        IsEnabled = true;
    }

    public int Id { get; }
    public bool IsEnabled { get; set; }
    public string? CurrentTicket { get; set; }
}
