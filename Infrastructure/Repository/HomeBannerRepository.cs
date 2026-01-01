using Applications.Dto;
using Applications.Interfaces.IRepository;
using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class HomeBannerRepository : IHomeBannerRepository
    {
        private readonly AppDbContext _db;

        public HomeBannerRepository(AppDbContext db)
        {
                _db=db;
        }
        public async Task AddAsync(HomeBannerDto dto)
        {
           var enitity = ToEntity(dto);

            _db.HomeBanners.Add(enitity);
            await _db.SaveChangesAsync();
            
        }

        public async Task DeleteAsync(int id)
        {
            var banner = await _db.HomeBanners.FindAsync(id);
            if (banner == null) return;

            _db.HomeBanners.Remove(banner);
            await _db.SaveChangesAsync();
        }

        public async Task<List<HomeBannerDto>> GetActiveAsync()
        {
            return await _db.HomeBanners
                .Where(b=>b.Status)
                .OrderBy(b=>b.Position)
                .Select(b=>ToDto(b))
                .ToListAsync();
        }

        public async Task<List<HomeBannerDto>> GetAllAsync()
        {
            return await _db.HomeBanners
                .OrderBy(b => b.Position)
                .Select(b => ToDto(b))
                .ToListAsync();
        }

        public async Task<HomeBannerDto?> GetByIdAsync(int id)
        {
            return await _db.HomeBanners
                .Where(b => b.Id == id)
                .Select(b => ToDto(b))
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(HomeBannerDto dto)
        {
            var banner = await _db.HomeBanners.FindAsync(dto.Id);
            if (banner == null) return;

            banner.Position = dto.Position;
            banner.Title = dto.Title;
            banner.Image = dto.Image;
            banner.EventId  = dto.EventId;
            banner.Status = dto.Status;
            banner.Link = dto.Link; 

            await _db.SaveChangesAsync();
        }

        private static HomeBannerDto ToDto(HomeBanner entity)
        {
            return new HomeBannerDto
            {
                Id = entity.Id,
                Image = entity.Image,
                Title = entity.Title,
                EventId = entity.EventId,
                Link = entity.Link,
                Position = entity.Position,
                Status = entity.Status
            };
        }

        private static HomeBanner ToEntity(HomeBannerDto dto)
        {
            return new HomeBanner
            {
                Image = dto.Image,
                Title = dto.Title,
                EventId = dto.EventId,
                Link = dto.Link,
                Position = dto.Position,
                Status = dto.Status
            };
        }
    }
}
