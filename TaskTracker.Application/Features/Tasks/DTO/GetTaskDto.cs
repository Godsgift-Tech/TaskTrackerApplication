using System.Text.Json.Serialization;

namespace TaskTracker.Application.Features.Tasks.DTO
{
    public class GetTaskDto
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int Status { get; set; }
        [JsonIgnore]

        public string AssignedToUserId { get; set; }
        public string UserName { get; set; } // optional
    }
}
