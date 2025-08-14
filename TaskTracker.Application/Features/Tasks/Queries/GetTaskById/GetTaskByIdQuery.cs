using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Core.Entity;

namespace TaskTracker.Application.Features.Tasks.Queries.GetTaskById
{
    public class GetTaskByIdQuery : IRequest<TaskItem>
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        // validate for manager
        public bool IsManager { get; set; }
    }
}
