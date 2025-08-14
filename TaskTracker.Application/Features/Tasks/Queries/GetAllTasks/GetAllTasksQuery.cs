using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Core.Entity;
using TaskTracker.Core.Helpers;

namespace TaskTracker.Application.Features.Tasks.Queries.GetAllTasks
{
    public class GetAllTasksQuery : IRequest<PagedList<TaskItem>>
    {
        public string? AssignedToUserId { get; set; }
        public bool IsManager { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

}
