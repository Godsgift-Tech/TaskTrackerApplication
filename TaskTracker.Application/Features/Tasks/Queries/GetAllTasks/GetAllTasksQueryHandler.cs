using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Common.Interfaces;
using TaskTracker.Core.Entity;
using TaskTracker.Core.Helpers;

namespace TaskTracker.Application.Features.Tasks.Queries.GetAllTasks
{
    public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, PagedList<TaskItem>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMemoryCache _cache;

        public GetAllTasksQueryHandler(ITaskRepository taskRepository, IMemoryCache cache)
        {
            _taskRepository = taskRepository;
            _cache = cache;
        }

        public async Task<PagedList<TaskItem>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Tasks_{request.AssignedToUserId}_{request.PageNumber}_{request.PageSize}_{request.FromDate}_{request.ToDate}_{request.IsManager}";

            if (_cache.TryGetValue(cacheKey, out PagedList<TaskItem> cachedTasks))
            {
                return cachedTasks;
            }

            PagedList<TaskItem> tasks;

            if (request.IsManager)
            {
                // Managers can see all tasks
                tasks = await _taskRepository.GetAllAsync(
                    userId: null,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    fromDate: request.FromDate,
                    toDate: request.ToDate
                );
            }
            else
            {
                // Regular users only see their own tasks
                tasks = await _taskRepository.GetAllAsync(
                    userId: request.AssignedToUserId,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    fromDate: request.FromDate,
                    toDate: request.ToDate
                );
            }

            _cache.Set(cacheKey, tasks, TimeSpan.FromMinutes(2));

            return tasks;
        }
    }
}
