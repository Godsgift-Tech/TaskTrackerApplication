using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Common.Interfaces;
using TaskTracker.Application.Features.Tasks.Queries.GetTaskById;
using TaskTracker.Application.Features.Tasks.Report;
using TaskTracker.Core.Entity;
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
        // Fetch tasks depending on role
        var tasks = request.IsManager
            ? await _taskRepository.QueryAll().ToListAsync(cancellationToken)
            : await _taskRepository.QueryByUserId(request.UserId).ToListAsync(cancellationToken);

        var total = tasks.Count;
        var completed = tasks.Count(t => t.Status == TaskStatus.Completed);
        var pending = tasks.Count(t => t.Status == TaskStatus.Pending);
        var inProgress = tasks.Count(t => t.Status == TaskStatus.InProgress);
        double completionRate = total == 0 ? 0 : (completed / (double)total) * 100;

        var taskDetails = tasks.Select(t => new TaskDetail
        {
            Id = t.Id,
            Title = t.Title,
            AssignedToUserId = t.AssignedToUserId,
            Status = (System.Threading.Tasks.TaskStatus)t.Status // Use your own TaskStatus enum
        }).ToList();

        return new TaskReport
        {
            TotalTasks = total,
            CompletedTasks = completed,
            PendingTasks = pending,
            InProgressTasks = inProgress,
            CompletionRate = completionRate,
            ReportGeneratedAt = DateTime.UtcNow,
            TaskDetails = taskDetails
        };
    }
}
