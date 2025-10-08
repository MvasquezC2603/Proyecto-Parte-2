namespace BasketApi.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string City { get; set; } = "";
        public string? LogoUrl { get; set; }

        public ICollection<Player> Players { get; set; } = new List<Player>();
        public ICollection<Match> HomeMatches { get; set; } = new List<Match>();
        public ICollection<Match> AwayMatches { get; set; } = new List<Match>();
    }
}
