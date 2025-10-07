namespace BasketApi.Models;
public class Player
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public int Number { get; set; }
    public string Position { get; set; } = "";
    public int HeightCm { get; set; }
    public int Age { get; set; }
    public string Nationality { get; set; } = "";
}
