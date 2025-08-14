using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Common.Interfaces;
using TaskTracker.Application.Features.Tasks.Command.DeleteCommand;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryCache _cache;

    public DeleteTaskCommandHandler(
        ITaskRepository taskRepository,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache)
    {
        _taskRepository = taskRepository;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return false;

        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = httpContext.User.FindFirstValue(ClaimTypes.Role);

        // Role check: manager can delete any task, otherwise only own tasks
        var deleted = role == "Manager"
            ? await _taskRepository.DeleteAsync(request.Id, null) // null = delete any task
            : await _taskRepository.DeleteAsync(request.Id, userId); // only own task

        if (!deleted) return false;

        // Remove from cache if exists
        _cache.Remove($"Task_{userId}_{request.Id}");

        return true;
    }
}
