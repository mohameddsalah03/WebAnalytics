using Analytics.Core.Domain.Contracts.Persistence;
using Analytics.Core.Domain.Entities;
using Analytics.Infrastructure.Persistence.Data;
using Analytics.Infrastructure.Persistence.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Persistence.Repositories;

internal class UserRepository : GenericRepository<User, int>, IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }


    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    

    public async Task<bool> IsEmailExistsAsync(string email)
        => await _context.Users
            .AnyAsync(u => u.Email == email);
    
}