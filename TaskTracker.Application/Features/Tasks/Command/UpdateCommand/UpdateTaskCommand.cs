using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Core.Entity;

namespace TaskTracker.Application.Features.Tasks.Command.UpdateCommand
{
    public class UpdateTaskCommand : IRequest<TaskItem>
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
