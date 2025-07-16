namespace TaskListApi.Dtos;

public class TaskItemDto
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
