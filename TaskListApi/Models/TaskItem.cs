namespace TaskListApi.Models;

public class TaskItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); 
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
