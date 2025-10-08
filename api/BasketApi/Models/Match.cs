namespace BasketApi.Models
{
    public class Match
    {
        public int Id { get; set; }

        public int HomeTeamId { get; set; }
        public Team? HomeTeam { get; set; }

        public int AwayTeamId { get; set; }
        public Team? AwayTeam { get; set; }

        public DateTime Kickoff { get; set; }

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public bool Finished { get; set; } = false;

        public ICollection<MatchRoster> Rosters { get; set; } = new List<MatchRoster>();
    }
}
