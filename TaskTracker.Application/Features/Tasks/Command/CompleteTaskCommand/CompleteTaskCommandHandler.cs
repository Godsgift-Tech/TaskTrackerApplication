using MediatR;
using Microsoft.Extensions.Caching.Memory;
using TaskTracker.Application.Features.Common.Interfaces;
using TaskStatus = TaskTracker.Core.Entity.TaskStatus;

namespace TaskTracker.Application.Features.Tasks.Command.CompleteTaskCommand
{
    public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, bool>
    {
        private readonly ITaskRepository _tasks;
        private readonly IMemoryCache _cache;

        public CompleteTaskCommandHandler(ITaskRepository tasks, IMemoryCache cache)
        {
            _tasks = tasks;
            _cache = cache;
        }

        public async Task<bool> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
        {
            // Managers can complete any task; users can complete only their own
            var task = request.IsManager
                ? await _tasks.GetByIdAsync(request.Id)
                : await _tasks.GetByIdAsync(request.Id, request.UserId);

            if (task is null) return false;

            // Idempotency: if already completed, succeed quietly
            if (task.Status == TaskStatus.Completed) return true;

            task.Status = TaskStatus.Completed;
            task.CompletedAt = DateTime.UtcNow;

            await _tasks.UpdateAsync(task);

            // Invalidate per-item cache
            _cache.Remove($"Task_{task.AssignedToUserId}_{task.Id}");
            return true;
        }
    }
}
