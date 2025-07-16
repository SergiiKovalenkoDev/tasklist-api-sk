using System.ComponentModel.DataAnnotations;

namespace TaskListApi.Dtos;

public class UpdateTaskListDto
{
    public string Name { get; set; } = null!;
    public string OwnerUserId { get; set; } = null!;
    public List<string> SharedUserIds { get; set; } = new();
    public List<TaskItemDto>? Tasks { get; set; }
}
