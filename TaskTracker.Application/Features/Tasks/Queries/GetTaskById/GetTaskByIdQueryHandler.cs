using MediatR;
using Microsoft.Extensions.Caching.Memory;
using TaskTracker.Application.Features.Common.Interfaces;
using TaskTracker.Application.Features.Tasks.DTO;

namespace TaskTracker.Application.Features.Tasks.Queries.GetTaskById
{
    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, GetTaskDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMemoryCache _cache;

        public GetTaskByIdQueryHandler(ITaskRepository taskRepository, IMemoryCache cache)
        {
            _taskRepository = taskRepository;
            _cache = cache;
        }

        //public async Task<TaskItem?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        //{
        //    return await _taskRepository.GetByIdAsync(request.Id, request.IsManager ? null : request.UserId);
        //    //  return await _taskRepository.GetByIdAsync(request.Id, request.UserId, request.IsManager);


        public async Task<GetTaskDto?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Task_{request.Id}_{request.UserId}_{request.IsManager}";

            if (_cache.TryGetValue(cacheKey, out GetTaskDto cachedTask))
                return cachedTask;

            var task = await _taskRepository.GetByIdAsync(
                request.Id,
                request.IsManager ? null : request.UserId,
                request.IsManager
            );

            if (task == null) return null;
           

            //  mapping
            var dto = new GetTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                CompletedAt = task.CompletedAt,
                Status = (int)task.Status,
                AssignedToUserId = task.AssignedToUserId,
                UserName = task.User != null ? task.User.FirstName : null

            };

            _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(2));

            return dto;
        }


    }
}
