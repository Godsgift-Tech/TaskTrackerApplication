using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Application.Features.Tasks.Command.CompleteTaskCommand;
using TaskTracker.Application.Features.Tasks.Command.CreateCommand;
using TaskTracker.Application.Features.Tasks.Command.DeleteCommand;
using TaskTracker.Application.Features.Tasks.Command.UpdateCommand;
using TaskTracker.Application.Features.Tasks.Queries.GetAllTasks;
using TaskTracker.Application.Features.Tasks.Queries.GetTaskById;
using TaskTracker.Core.Entity;

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
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskCommand command)
        {
            // Get logged-in user's ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            // Checking route ID matches the command ID
            if (id != command.Id)
                return BadRequest("Task ID mismatch.");

            // Attach user ID from claims
            command.UserId = userId;

            var result = await _mediator.Send(command);

            if (result == null)
            {
                // Check if task exists at all for better feedback
                return Forbid("You are not authorized to update this task or it does not exist.");
            }

            return Ok(result);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //  JWT or identity claims

            var report = await _mediator.Send(new GetTaskCompletionReportQuery(userId));

            return Ok(report);
        }

        // Get all tasks for the logged-in user
        [HttpGet]
        [Authorize(Roles = "User,Manager")]
        public async Task<IActionResult> GetUserTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new GetAllTasksQuery { AssignedToUserId = userId! }; 
            var tasks = await _mediator.Send(query);
            return Ok(tasks);
        }


        // Delete task - only if owned by the user or manager
        [HttpDelete("{id}")]
        [Authorize(Roles = "User,Manager")]
       
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var deleted = await _mediator.Send(new DeleteTaskCommand { Id = id });
            if (!deleted)
                return Forbid("You are not authorized to delete this task or it does not exist.");

            return NoContent();
        }



    }
}
