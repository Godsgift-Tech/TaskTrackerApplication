using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Tasks.DTO;
using TaskTracker.Core.Entity;
using TaskTracker.Core.Helpers;

namespace TaskTracker.Application.Features.Tasks.Queries.GetAllTasks
{
    public class GetAllTasksQuery : IRequest<PagedList<TaskDto>>, IBaseRequest
    {
        public string? AssignedToUserId { get; set; }
        public bool IsManager { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

}
