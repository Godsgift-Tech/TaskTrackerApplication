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
        // Task<PagedList<TaskItem>> GetAllAsync(string userId, int pageNumber, int pageSize, DateTime? fromDate = null, DateTime? toDate = null);
        Task<PagedList<TaskItem>> GetAllAsync(
     string? userId = null,
     int pageNumber = 1,
     int pageSize = 10,
     DateTime? fromDate = null,
     DateTime? toDate = null
 );

        Task<List<TaskItem>> GetAllAsync(string? userId = null);
        Task<List<TaskItem>> GetTasksByUserIdAsync(string userId);
       // Task<List<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetByIdAsync(Guid id, string? userId = null, bool isManager = false);

        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task<bool> DeleteAsync(Guid id, string? userId);

    }
}
