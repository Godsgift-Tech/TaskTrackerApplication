using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var cacheKey = $"Tasks_{request.AssignedToUserId}_{request.PageNumber}_{request.PageSize}_{request.FromDate}_{request.ToDate}";

            if (_cache.TryGetValue(cacheKey, out PagedList<TaskItem> cachedTasks))
            {
                return cachedTasks;
            }

            var tasks = await _taskRepository.GetAllAsync(
                request.AssignedToUserId,
                request.PageNumber,
                request.PageSize,
                request.FromDate,
                request.ToDate
            );

            _cache.Set(cacheKey, tasks, TimeSpan.FromMinutes(2));

            return tasks;
        }
    }
}
