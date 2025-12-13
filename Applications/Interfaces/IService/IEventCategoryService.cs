using Applications.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Interfaces.IService
{
    public interface IEventCategoryService
    {
        Task<IEnumerable<EventCategoryDto>> GetAllAsync();
        Task<EventCategoryDto?> GetByIdAsync(int id);
        Task <EventCategoryDto>CreateAsync(EventCategoryDto dto);
        Task UpdateAsync(int id,EventCategoryDto dto);
        Task DeleteAsync(int id);
        
    }
}
