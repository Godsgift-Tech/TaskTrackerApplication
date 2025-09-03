using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Tasks.Command.CreateCommand.CreateCommandResponse;

namespace TaskTracker.Application.Features.Tasks.Command.CreateCommand
{
    public class CreateTaskCommand : IRequest<CreateTaskResponseDto>
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime DueDate { get; set; }
        public string AssignedToUserId { get; set; } = default!;
    }
}
