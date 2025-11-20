namespace Applications.Dto.OrganizerDto
{
    public class ManagerLoginResponseDto
    {
        public bool ChangePassword { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public int UserId { get; set; }
        public string? Role { get; set; }
    }

}
