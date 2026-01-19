using Applications.Dto.UserDto;
using Microsoft.AspNetCore.Http;

namespace Applications.Interfaces.IService
{
    public interface IRealUserService
    {
        Task<RealUserDto> GetUserAsync(int userId);
        Task<string> UpdateUserAsync(int userId, RealUserDto dto);
        Task<string> UploadProfileImageAsync(int userId, IFormFile file);
    }
}
