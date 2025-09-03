using MediatR;
using TaskTracker.Application.Features.Common.Interfaces;
using TaskTracker.Application.Features.Tasks.Command.CreateCommand.CreateCommandResponse;
using TaskTracker.Core.Entity;

namespace TaskTracker.Application.Features.Tasks.Command.CreateCommand
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, CreateTaskResponseDto>
    {
        private readonly ITaskRepository _taskRepository;

        public CreateTaskCommandHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<CreateTaskResponseDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var taskItem = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                CreatedAt = DateTime.UtcNow,
                AssignedToUserId = request.AssignedToUserId,
            };

            await _taskRepository.AddAsync(taskItem);

            // Reload User to get username
            await _taskRepository.ReloadUserAsync(taskItem);

            return new CreateTaskResponseDto
            {
                Message = "Task creation was successful! And task is assigned to:  " + taskItem.User?.UserName
              
            };
        }
    }
}
