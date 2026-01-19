using Applications.Interfaces.IRepository;
using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RealUserRepository : IRealUserRepository
    {
        private readonly AppDbContext _context;

        public RealUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RealUser?> GetRealUserAsync(int userId)
        {
            return await _context.RealUsers
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId);
        }

        public async Task CreateAsync(RealUser user)
        {
            await _context.RealUsers.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
