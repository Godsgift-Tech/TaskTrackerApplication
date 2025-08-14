namespace TaskTracker.Application.Features.Tasks.Report
{
    public class TaskDetail
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string AssignedToUserId { get; set; } = null!;
        public TaskStatus Status { get; set; }
    }

}
