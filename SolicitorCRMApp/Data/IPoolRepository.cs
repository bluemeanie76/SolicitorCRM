using SolicitorCRMApp.Models;

namespace SolicitorCRMApp.Data;

public interface IPoolRepository
{
    Task<IReadOnlyList<Pool>> GetAllAsync();
    Task<int> CreateAsync(Pool pool);
    Task UpdateAsync(Pool pool);
    Task SetEnabledAsync(int poolId, bool enabled);
    Task AssignUserAsync(int poolId, int userId);
    Task RemoveUserAsync(int poolId, int userId);
    Task<IReadOnlyList<User>> GetUsersAsync(int poolId);
}
