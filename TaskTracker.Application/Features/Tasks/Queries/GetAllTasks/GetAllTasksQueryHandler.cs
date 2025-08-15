using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Common.Interfaces;
using TaskTracker.Application.Features.Tasks.DTO;
using TaskTracker.Core.Entity;
using TaskTracker.Core.Helpers;

namespace TaskTracker.Application.Features.Tasks.Queries.GetAllTasks
{
    public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, PagedList<TaskDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMemoryCache _cache;

        public GetAllTasksQueryHandler(ITaskRepository taskRepository, IMemoryCache cache)
        {
            _taskRepository = taskRepository;
            _cache = cache;
        }

        public async Task<PagedList<TaskDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Tasks_{request.AssignedToUserId}_{request.PageNumber}_{request.PageSize}_{request.FromDate}_{request.ToDate}_{request.IsManager}";

            if (_cache.TryGetValue(cacheKey, out PagedList<TaskDto> cachedTasks))
            {
                return cachedTasks;
            }

            IQueryable<TaskItem> query;

            // Use the new repository query methods
            query = request.IsManager
                ? _taskRepository.QueryAll()
                : _taskRepository.QueryByUserId(request.AssignedToUserId);

            // Apply date filters if provided
            if (request.FromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(t => t.CreatedAt <= request.ToDate.Value);

            // Order descending by creation date
            query = query.OrderByDescending(t => t.CreatedAt);

            // Project to DTO to avoid JSON cycles
            var projectedQuery = query.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                CompletedAt = t.CompletedAt,
                Status = (int)t.Status,
                AssignedToUserId = t.AssignedToUserId,
                UserName = t.User != null ? t.User.FirstName : null
            });

            // Create paged result
            var pagedTasks = await PagedList<TaskDto>.CreateAsync(projectedQuery, request.PageNumber, request.PageSize);

            // Cache for 2 minutes
            _cache.Set(cacheKey, pagedTasks, TimeSpan.FromMinutes(2));

            return pagedTasks;
        }
    }
}
