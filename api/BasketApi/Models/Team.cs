namespace BasketApi.Models;
public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = "";
    public string LogoUrl { get; set; } = "";
    public List<Player> Players { get; set; } = new();
}
