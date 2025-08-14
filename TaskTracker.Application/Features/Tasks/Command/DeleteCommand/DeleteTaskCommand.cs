using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Tasks.Command.DeleteCommand
{
    public class DeleteTaskCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
    }
}
