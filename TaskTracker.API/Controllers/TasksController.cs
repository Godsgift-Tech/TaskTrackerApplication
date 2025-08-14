using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Application.Features.Tasks.Command.CompleteTaskCommand;
using TaskTracker.Application.Features.Tasks.Command.CreateCommand;
using TaskTracker.Application.Features.Tasks.Command.UpdateCommand;
using TaskTracker.Application.Features.Tasks.Queries.GetAllTasks;
using TaskTracker.Application.Features.Tasks.Queries.GetTaskById;

namespace TaskTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Create task - any authenticated user
        [HttpPost]
        [Authorize(Roles = "User,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateTaskCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            command.AssignedToUserId = userId!;

            var taskId = await _mediator.Send(command);
            return Ok(new { Id = taskId });
        }

        // Update task - only if task belongs to user
        [HttpPut("{id}")]
        [Authorize(Roles = "User,Manager")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            command.Id = id;
            command.UserId = userId!;

            var updated = await _mediator.Send(command);
            if (!updated) return Forbid("You are not authorized to update this task.");

            return NoContent();
        }

        // Complete task - only if task belongs to user
        [HttpPatch("{id}/complete")]
        [Authorize(Roles = "User,Manager")]
        public async Task<IActionResult> Complete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new CompleteTaskCommand { Id = id, UserId = userId! };

            var completed = await _mediator.Send(command);
            if (!completed) return Forbid("You are not authorized to complete this task.");

            return NoContent();
        }

        // Get task by ID - any authenticated user if they own it, manager can see all
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var query = new GetTaskByIdQuery { Id = id, UserId = userId!, IsManager = role == "Manager" };
            var task = await _mediator.Send(query);

            if (task == null) return NotFound();
            return Ok(task);
        }

        // Get all tasks - Managers only
        [HttpGet("all")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _mediator.Send(new GetAllTasksQuery());
            return Ok(tasks);
        }
        [HttpGet("reports/completion")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetCompletionReport()
        {
            var report = await _mediator.Send(new GetTaskCompletionReportQuery());
            return Ok(report);
        }

        [HttpPatch("{id}/complete")]
        [Authorize(Roles = "User,Manager")]
        public async Task<IActionResult> Complete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isManager = User.IsInRole("Manager");

            var cmd = new CompleteTaskCommand
            {
                Id = id,
                UserId = userId!,
                IsManager = isManager
            };

            var ok = await _mediator.Send(cmd);
            if (!ok) return NotFound(new { message = "Task not found or access denied." });

            return NoContent();
        }

    }
}
