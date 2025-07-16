using System.ComponentModel.DataAnnotations;

namespace TaskListApi.Dtos;

public class CreateTaskListDto
{
    public string Name { get; set; } = null!;
    public string OwnerUserId { get; set; } = null!;

    public List<string> SharedUserIds { get; set; } = new();
    public DateTime CreatedAt { get; set; }

    public List<TaskItemDto>? Tasks { get; set; }
}
