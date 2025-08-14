using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Tasks.Command.CompleteTaskCommand
{
    public class CompleteTaskCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = default!;
        public bool IsManager { get; set; }  // set from User.IsInRole("Manager") in controller
    }
}
