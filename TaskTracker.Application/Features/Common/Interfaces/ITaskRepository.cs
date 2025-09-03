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
        IQueryable<TaskItem> QueryAll();
        IQueryable<TaskItem> QueryByUserId(string userId);
        Task<PagedList<TaskItem>> GetAllAsync(string? userId = null, int pageNumber = 1, int pageSize = 10, DateTime? fromDate = null, DateTime? toDate = null);
        Task<TaskItem?> GetByIdAsync(Guid id, string? userId = null, bool isManager = false);
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task ReloadUserAsync(TaskItem task);

        Task<bool> DeleteAsync(Guid id, string? userId);
    }

}
