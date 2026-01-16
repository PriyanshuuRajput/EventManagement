using Applications.Dto;
using Applications.Dto.OrganizerDto;
using Microsoft.AspNetCore.Http;

namespace Applications.Interfaces.IService
{
    public interface IManagerService
    {
        Task<string> SignUpManagerAsync(ManagerSignUpDto dto);

        Task<string> ApproveManagerAsync(int managerId);
        Task<List<ManagerProfileDto>> GetPendingManagersAsync();
        Task<List<ManagerProfileDto>> GetAllManagersAsync();

        Task RejectManagerAsync(int managerId, string reason);

        Task<string> ManagerProfileChangePassword(int userId, ManagerProfileDto dto);
        Task<bool> DeleteManagerAsync(int managerId);
        Task<string> UploadProfileImageAsync(int userId, IFormFile image);
        Task <string> UpdateManagerProfileAsync(int userId , ManagerAccountDto dto);
    }
}
