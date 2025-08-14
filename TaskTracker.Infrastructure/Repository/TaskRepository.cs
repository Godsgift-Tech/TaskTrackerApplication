using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Common.Interfaces;
using TaskTracker.Core.Entity;
using TaskTracker.Core.Helpers;
using TaskTracker.Infrastructure.Data;

namespace TaskTracker.Infrastructure.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskTrackerContext _context;

        public TaskRepository(TaskTrackerContext context) => _context = context;

        public async Task<PagedList<TaskItem>> GetAllAsync(
      string? userId = null,
      int pageNumber = 1,
      int pageSize = 10,
      DateTime? fromDate = null,
      DateTime? toDate = null)
        {
            // Include the related User entity
            var query = _context.Tasks
                .Include(t => t.User) // Eager load user info
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(t => t.AssignedToUserId == userId);
            }

            if (fromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.CreatedAt <= toDate.Value);

            query = query.OrderByDescending(t => t.CreatedAt);

            // Use CreateAsync to return a PagedList with proper pagination
            return await PagedList<TaskItem>.CreateAsync(query, pageNumber, pageSize);
        }




        public async Task<List<TaskItem>> GetAllAsync(string? userId = null)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Where(t => userId == null || t.AssignedToUserId == userId)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id, string? userId = null, bool isManager = false)
        {
            if (isManager)
            {
                // Managers can access any task
                return await _context.Tasks.AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id);
            }

            // Regular users only see their own task
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _context.Tasks.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.AssignedToUserId == userId);
        }

      
        public async Task<List<TaskItem>> GetTasksByUserIdAsync(string userId)
        {
            return await _context.Tasks
                .Where(t => t.AssignedToUserId == userId)
                .ToListAsync();
        }
       
        public async Task AddAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id, string? userId)
        {
            var task = userId == null
                ? await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id)
                : await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.AssignedToUserId == userId);

            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
