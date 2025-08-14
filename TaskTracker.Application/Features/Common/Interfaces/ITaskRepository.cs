using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Core.Entity;
using TaskTracker.Core.Helpers;

namespace TaskTracker.Application.Features.Common.Interfaces
{
    public interface ITaskRepository
    {
        Task<PagedList<TaskItem>> GetAllAsync(string userId, int pageNumber, int pageSize, DateTime? fromDate = null, DateTime? toDate = null);
        Task<TaskItem?> GetByIdAsync(Guid id, string userId);
        Task<TaskItem?> GetByIdAnyAsync(Guid id); 
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task<bool> DeleteAsync(Guid id, string userId);
    }
}
