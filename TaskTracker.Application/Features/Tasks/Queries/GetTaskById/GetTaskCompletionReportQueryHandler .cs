using MediatR;
using TaskTracker.Application.Features.Common.Interfaces;
using TaskTracker.Application.Features.Tasks.Queries.GetTaskById;
using TaskTracker.Application.Features.Tasks.Report;
using TaskTracker.Core.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskStatus = TaskTracker.Core.Entity.TaskStatus;

public class GetTaskCompletionReportQueryHandler
    : IRequestHandler<GetTaskCompletionReportQuery, TaskReport>
{
    private readonly ITaskRepository _taskRepository;

    public GetTaskCompletionReportQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskReport> Handle(GetTaskCompletionReportQuery request, CancellationToken cancellationToken)
    {
        var userTasks = await _taskRepository.GetTasksByUserIdAsync(request.UserId);

        var total = userTasks.Count;
        var completed = userTasks.Count(t => t.Status == TaskStatus.Completed);
        var pending = userTasks.Count(t => t.Status == TaskStatus.Pending);
        var inProgress = userTasks.Count(t => t.Status == TaskStatus.InProgress);

        return new TaskReport
        {
            TotalTasks = total,
            CompletedTasks = completed,
            PendingTasks = pending,
            InProgressTasks = inProgress
        };
    }
}
