namespace TaskListApi.Dtos;

public class TaskListDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string OwnerUserId { get; set; } = null!;
    public List<string> SharedUserIds { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<TaskItemDto> Tasks { get; set; } = new();
}
