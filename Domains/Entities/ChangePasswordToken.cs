namespace Domains.Entities
{
    public class ChangePasswordToken
    {
        public int Id { get; set; }

        public int AdminUserId { get; set; }   

        public string Token { get; set; } = string.Empty;

        public DateTime Expiry { get; set; }

        public bool IsUsed { get; set; }

        public AdminUser AdminUser { get; set; } = null!;
    }
}
