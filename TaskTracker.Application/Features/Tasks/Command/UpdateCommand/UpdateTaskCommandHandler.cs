using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Common.Interfaces;
using TaskTracker.Application.Features.Tasks.DTO;
using TaskTracker.Core.Entity;

namespace TaskTracker.Application.Features.Tasks.Command.UpdateCommand
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMemoryCache _cache;

        public UpdateTaskCommandHandler(ITaskRepository taskRepository, IMemoryCache cache)
        {
            _taskRepository = taskRepository;
            _cache = cache;
        }

        public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            // Only managers can update tasks
            if (!request.IsManager)
                return null;

            // Fetch task with User included (tracked)
            var task = await _taskRepository.GetByIdAsync(request.Id, null, true);
            if (task == null)
                return null;

            // Update task properties
            task.Title = request.Title;
            task.Description = request.Description;
            task.DueDate = request.DueDate ?? task.DueDate;

            // Reassign if provided
            if (!string.IsNullOrEmpty(request.AssignedToUserId))
            {
                task.AssignedToUserId = request.AssignedToUserId;

                // Reset status and CompletedAt for the new user
                task.Status = 0;            // default status (Pending)
                task.CompletedAt = null;    // clear completed date
            }

            // Save changes
            await _taskRepository.UpdateAsync(task);

            // Reload User navigation to reflect reassignment
            await _taskRepository.ReloadUserAsync(task);

            // Clear cache
            _cache.Remove($"Task_{task.AssignedToUserId}_{task.Id}");

            // Map to DTO and return
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                CompletedAt = task.CompletedAt,
                Status = (int)task.Status,
                AssignedToUserId = task.AssignedToUserId,
                UserName = task.User?.UserName // fresh reassigned user
            };
        }
    }
}
