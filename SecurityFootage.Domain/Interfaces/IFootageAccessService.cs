using SecurityFootage.Domain.Entities;
using SecurityFootage.Domain.Results;

namespace SecurityFootage.Domain.Interfaces
{
    public interface IFootageAccessService
    {
        Task<CreateUserResult> CreateUserAsync(User createUser, string nickName, CancellationToken ct);
        Task<FootageAccessResponse> RequestFootageAccessAsync(Location location, string nickName, CancellationToken ct);
        Task<User?> GetUserByNicknameAsync(string nickName, CancellationToken ct);
    }
}