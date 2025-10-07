namespace BasketApi.Models
{
    public class AdminUserProfile
    {
        public int Id { get; set; }

        // FK al usuario Identity
        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        // Campos extra de tu Admin
        public string? Notes { get; set; }
    }
}
