using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Application.Features.Tasks.Command.CompleteTaskCommand;
using TaskTracker.Application.Features.Tasks.Command.CreateCommand;
using TaskTracker.Application.Features.Tasks.Command.DeleteCommand;
using TaskTracker.Application.Features.Tasks.Command.UpdateCommand;
using TaskTracker.Application.Features.Tasks.DTO;
using TaskTracker.Application.Features.Tasks.Queries.GetAllTasks;
using TaskTracker.Application.Features.Tasks.Queries.GetTaskById;
using TaskTracker.Core.Entity;
using TaskTracker.Core.Helpers;

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

        [HttpPost]
        [Authorize(Roles = "User,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateTaskCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role != "Manager")
            {
                // Regular users can only assign tasks to themselves
                command.AssignedToUserId = userId!;
            }
            else
            {
                // Managers can assign to anyone
                if (string.IsNullOrEmpty(command.AssignedToUserId))
                    command.AssignedToUserId = userId; // fallback to self if no user specified
            }

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

            if (!completed)
                return Forbid("You are not authorized to complete this task.");

            return Ok("Task completed successfully");
        }


        // Get task by ID - any authenticated user if they own it, manager can see all
        [HttpGet("{id}")]
        [Authorize(Roles = "User,Manager")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);
            var isManager = role == "Manager";

            var query = new GetTaskByIdQuery
            {
                Id = id,
                UserId = isManager ? null : userId, // Managers can access any task
                IsManager = isManager
            };

            var task = await _mediator.Send(query);

            if (task == null)
                return NotFound("Task not found or you do not have access.");

            return Ok(task);
        }



        [HttpGet("reports/completion")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetTaskCompletionReport()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            // Pass values via constructor
            var query = new GetTaskCompletionReportQuery(userId, role == "Manager");

            var report = await _mediator.Send(query);
            return Ok(report);
        }


        // Get all tasks for the logged-in user
        [HttpGet("allTasks")]
        [Authorize(Roles = "User,Manager")]
        public async Task<ActionResult<PagedList<TaskDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            // Get current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Determine if the user is a manager (based on role claim)
            var isManager = User.IsInRole("Manager");

            var query = new GetAllTasksQuery
            {
                AssignedToUserId = userId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                FromDate = fromDate,
                ToDate = toDate,
                IsManager = isManager
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // Delete task - only if owned by the user or manager
        [HttpDelete("{id}")]
        [Authorize(Roles = "User,Manager")]
       
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var deleted = await _mediator.Send(new DeleteTaskCommand { Id = id });
            if (!deleted)
                return Forbid("You are not authorized to delete this task or it does not exist.");

            return Ok("Task removed successfully!");

        }



    }
}
