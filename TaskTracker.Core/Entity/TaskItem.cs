using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TaskTracker.Core.Entity
{
    // using enum for task status
    public enum TaskStatus { Pending, InProgress, Completed }
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        // Foreign Key to User
        public string AssignedToUserId { get; set; } = default!;

        // Navigation Property
        public User User { get; set; } = default!;
    }
}
