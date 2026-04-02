namespace ControlFilas2026.Models;

public class Ticket
{
    public Ticket(string number, bool isSpecial)
    {
        Number = number;
        IsSpecial = isSpecial;
    }

    public string Number { get; }
    public bool IsSpecial { get; }
}
