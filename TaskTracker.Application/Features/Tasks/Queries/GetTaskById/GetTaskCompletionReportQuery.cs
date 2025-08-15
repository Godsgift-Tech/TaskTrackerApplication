using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Tasks.Report;

namespace TaskTracker.Application.Features.Tasks.Queries.GetTaskById
{
    public class GetTaskCompletionReportQuery : IRequest<TaskReport>
    {
        public string UserId { get; }
        public bool IsManager { get; }

        public GetTaskCompletionReportQuery(string userId, bool isManager)
        {
            UserId = userId;
            IsManager = isManager;
        }
    }

}
