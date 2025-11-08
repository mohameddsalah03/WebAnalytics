using Analytics.Core.Domain.Entities;

namespace Analytics.Core.Domain.Contracts.Persistence;

public interface IUserRepository : IGenericRepository<User, int>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> IsEmailExistsAsync(string email);
}