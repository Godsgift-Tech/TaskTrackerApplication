using Microsoft.AspNetCore.Identity;
using System;

namespace TaskTracker.Core.Entity
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}
