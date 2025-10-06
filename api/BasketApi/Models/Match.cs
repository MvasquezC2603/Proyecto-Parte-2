namespace BasketApi.Models
{
    public class Match
    {
        public int Id { get; set; }
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public DateTime StartAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndAt { get; set; }
        public int Quarter { get; set; } = 1;
        public int ScoreHome { get; set; } = 0;
        public int ScoreAway { get; set; } = 0;
        public int FoulsHome { get; set; } = 0;
        public int FoulsAway { get; set; } = 0;
        public string Status { get; set; } = "finished";
    }
}
