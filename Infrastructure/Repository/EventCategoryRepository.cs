using Applications.Interfaces.IRepository;
using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class EventCategoryRepository : IEventCategoryRepository
    {
        private readonly AppDbContext _context;
        public EventCategoryRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<EventCategory>> GetAllAsync()
        {
            return await _context.EventCategories.ToListAsync();
        }

        public async Task<EventCategory?> GetByIdAsync(int id)
        {
            return await _context.EventCategories.FindAsync(id);

        }

        public async Task<EventCategory> AddAsync(EventCategory category)
        { 
            await _context.EventCategories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.EventCategories.FindAsync(id);
                if (category != null)
            {
                _context.Remove(category);
            }
                await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EventCategory category)
        {
             _context.EventCategories.Update(category);
            await _context.SaveChangesAsync();

        }

       
    }
}
