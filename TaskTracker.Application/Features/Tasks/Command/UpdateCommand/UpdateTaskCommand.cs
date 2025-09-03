using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Tasks.DTO;
using TaskTracker.Core.Entity;

namespace TaskTracker.Application.Features.Tasks.Command.UpdateCommand
{
    public class UpdateTaskCommand : IRequest<TaskDto>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string AssignedToUserId { get; set; } = default!;
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        [JsonIgnore]
        public bool IsManager { get; set; }

    }
}
