using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Common.Interfaces;
using TaskTracker.Core.Entity;

namespace TaskTracker.Application.Features.Tasks.Queries.GetTaskById
{
    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskItem>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMemoryCache _cache;

        public GetTaskByIdQueryHandler(ITaskRepository taskRepository, IMemoryCache cache)
        {
            _taskRepository = taskRepository;
            _cache = cache;
        }

        public async Task<TaskItem> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Task_{request.UserId}_{request.Id}";

            if (_cache.TryGetValue(cacheKey, out TaskItem cachedTask))
            {
                return cachedTask;
            }

            var task = await _taskRepository.GetByIdAsync(request.Id, request.UserId);
            if (task != null)
            {
                _cache.Set(cacheKey, task, TimeSpan.FromMinutes(2));
            }

            return task;
        }
    }
}
