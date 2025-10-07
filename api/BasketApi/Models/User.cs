namespace BasketApi.Models;
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!; // RNF: almacenar con hash
    public string Role { get; set; } = "admin";
}
