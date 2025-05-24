using ReftLabsTask.Internal.Core.Models;

namespace ReftLabsTask.Internal.Core;
public interface IExternalUserService
{
    Task<User> GetUserByIdAsync(int userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
}
