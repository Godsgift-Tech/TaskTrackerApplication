using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Common.Interfaces;
using TaskTracker.Core.Entity;

namespace TaskTracker.Application.Features.Tasks.Command.UpdateCommand
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskItem>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMemoryCache _cache;

        public UpdateTaskCommandHandler(ITaskRepository taskRepository, IMemoryCache cache)
        {
            _taskRepository = taskRepository;
            _cache = cache;
        }

        public async Task<TaskItem> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            // Retrieve task only if it belongs to the given user
            var existingTask = await _taskRepository.GetByIdAsync(request.Id, request.AssignedToUserId);
            if (existingTask == null)
                return null;

            // Update fields
            existingTask.Title = request.Title;
            existingTask.Description = request.Description;
            // keep old date
            existingTask.DueDate = request.DueDate ?? existingTask.DueDate; 

            await _taskRepository.UpdateAsync(existingTask);

            // Invalidate cache to ensure fresh data on next retrieval
            _cache.Remove($"Task_{request.AssignedToUserId}_{request.Id}");

            return existingTask;
        }
    }
}
