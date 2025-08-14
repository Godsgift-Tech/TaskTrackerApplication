using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var existingTask = await _taskRepository.GetByIdAsync(request.Id, request.UserId);
            if (existingTask == null) return null;

            existingTask.Title = request.Title;
            existingTask.Description = request.Description;
            existingTask.DueDate = (DateTime)request.DueDate;

            await _taskRepository.UpdateAsync(existingTask);

            // Invalidate cache
            _cache.Remove($"Task_{request.UserId}_{request.Id}");

            return existingTask;
        }
    }
}
