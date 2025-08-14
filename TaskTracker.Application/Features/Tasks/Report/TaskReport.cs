using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Tasks.Report
{
    public class TaskReport
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public double CompletionRate { get; set; }
        public DateTime ReportGeneratedAt { get; set; }

        public List<TaskDetail> TaskDetails { get; set; } = new();
    }

}
