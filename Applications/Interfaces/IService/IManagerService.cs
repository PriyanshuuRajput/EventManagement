using Applications.Dto.OrganizerDto;

namespace Applications.Interfaces.IService
{
    public interface IManagerService
    {
        Task<string> SignUpManagerAsync(ManagerSignUpDto dto);

        Task<string> ApproveManagerAsync(int managerId);
        Task<List<object>> GetPendingManagersAsync();

        Task RejectManagerAsync(int managerId);

    }
}
