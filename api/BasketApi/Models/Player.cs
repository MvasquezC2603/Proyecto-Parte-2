namespace BasketApi.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public int Number { get; set; }
        public string Position { get; set; } = "";
        public double Height { get; set; }
        public int Age { get; set; }
        public string Nationality { get; set; } = "";

        public int TeamId { get; set; }           // FK correcta
        public Team? Team { get; set; }           // navegación
    }
}
