using Microsoft.AspNetCore.Http;

namespace Applications.Dto.OrganizerDto
{
    public class ProfileImageUploadDto
    {
        public IFormFile Image { get; set; } = default!;
    }
}
