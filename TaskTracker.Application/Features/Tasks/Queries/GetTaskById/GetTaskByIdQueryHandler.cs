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

        public async Task<TaskItem?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            return await _taskRepository.GetByIdAsync(request.Id, request.IsManager ? null : request.UserId);
        }
    }
}
