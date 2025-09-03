using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Common.Interfaces;
using TaskTracker.Application.Features.Tasks.DTO;
using TaskTracker.Core.Entity;
using TaskTracker.Core.Helpers;
using TaskTracker.Infrastructure.Data;

namespace TaskTracker.Infrastructure.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskTrackerContext _context;

        public TaskRepository(TaskTrackerContext context) => _context = context;

        public IQueryable<TaskItem> QueryAll()
        {
            // Return IQueryable so handler can use  DTO to filter further
            return _context.Tasks.Include(t => t.User).AsQueryable();
        }

        public IQueryable<TaskItem> QueryByUserId(string userId)
        {
            return _context.Tasks
                .Include(t => t.User)
                .Where(t => t.AssignedToUserId == userId)
                .AsQueryable();
        }
        public async Task<PagedList<TaskItem>> GetAllAsync(
            string? userId = null,
            int pageNumber = 1,
            int pageSize = 10,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _context.Tasks.Include(t => t.User).AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(t => t.AssignedToUserId == userId);

            if (fromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.CreatedAt <= toDate.Value);

            query = query.OrderByDescending(t => t.CreatedAt);

            return await PagedList<TaskItem>.CreateAsync(query, pageNumber, pageSize);
        }

        //    public async Task<TaskItem?> GetByIdAsync(Guid id, string? userId = null, bool isManager = false)
        //    {
        //        if (isManager)
        //            return await _context.Tasks
        //                .AsNoTracking()
        //                .FirstOrDefaultAsync(t => t.Id == id);
        //        return await _context.Tasks

        //.AsNoTracking()
        //.FirstOrDefaultAsync();


        //        if (string.IsNullOrEmpty(userId)) return null;

        //        return await _context.Tasks.AsNoTracking()
        //            .FirstOrDefaultAsync(t => t.Id == id && t.AssignedToUserId == userId);
        //    }

        //public async Task<TaskItem?> GetByIdAsync(Guid id, string? userId = null, bool isManager = false)
        //{
        //    IQueryable<TaskItem> query = _context.Tasks.AsNoTracking()
        //       .Include(t=>t.User) ;

        //    if (isManager)
        //    {
        //        // Manager can fetch by Id only
        //        return await query.FirstOrDefaultAsync(t => t.Id == id);
        //    }

        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        // If not manager and no userId provided → invalid
        //        return null;
        //    }

        //    // Regular user must match both Id and AssignedToUserId
        //    return await query.FirstOrDefaultAsync(t => t.Id == id && t.AssignedToUserId == userId);
        //}


        public async Task<TaskItem?> GetByIdAsync(Guid id, string? userId = null, bool isManager = false)
        {
            IQueryable<TaskItem> query = _context.Tasks
               .Include(t => t.User);

            if (isManager)
                return await query.FirstOrDefaultAsync(t => t.Id == id);

            if (string.IsNullOrEmpty(userId))
                return null;

            return await query.FirstOrDefaultAsync(t => t.Id == id && t.AssignedToUserId == userId);
        }

        public async Task<List<TaskItem>> GetAllAsync(string? userId = null)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Where(t => userId == null || t.AssignedToUserId == userId)
                .ToListAsync();
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
        public async Task ReloadUserAsync(TaskItem task)
        {
            await _context.Entry(task).Reference(t => t.User).LoadAsync();
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
