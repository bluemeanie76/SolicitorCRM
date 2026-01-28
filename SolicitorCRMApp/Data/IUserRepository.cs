using SolicitorCRMApp.Models;

namespace SolicitorCRMApp.Data;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<IReadOnlyList<UserType>> GetUserTypesAsync();
    Task<int> CreateAsync(UserFormViewModel model, string passwordHash, string passwordSalt);
    Task UpdateAsync(UserFormViewModel model, string? passwordHash, string? passwordSalt);
    Task SetEnabledAsync(int id, bool enabled);
}
