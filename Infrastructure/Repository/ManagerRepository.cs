using Applications.Interfaces.IRepository;
using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly AppDbContext _db;

        public ManagerRepository(AppDbContext db)
        {
            _db = db;
        }

        //  Correct name
        public async Task<bool> EmailExists(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<AdminUser> CreateAdminUser(AdminUser user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<Manager> CreateManager(Manager manager)
        {
            _db.Managers.Add(manager);
            await _db.SaveChangesAsync();
            return manager;
        }

        //  Basic fetch
        public async Task<Manager?> GetManagerByIdAsync(int id)
        {
            return await _db.Managers
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        //  Needed for manager approval (includes AdminUser)
        public async Task<Manager?> GetManagerWithUserAsync(int id)
        {
            return await _db.Managers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        //  Corrected method name
        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }

        public async Task<List<Manager>> GetPendingManagersAsync()
        {
            return await _db.Managers
                .Include(m => m.User)
                .Where(m => !m.IsApproved)
                .ToListAsync();
        }

        public void DeleteManager(Manager manager)
        {
            if (manager.User != null)
                _db.Users.Remove(manager.User);

            // Then remove manager
            _db.Managers.Remove(manager);
        }
    }
}
