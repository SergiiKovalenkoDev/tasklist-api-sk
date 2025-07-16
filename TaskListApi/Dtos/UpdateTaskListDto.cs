using System.ComponentModel.DataAnnotations;

namespace TaskListApi.Dtos;

public class UpdateTaskListDto
{
    [Required]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 255 characters.")]
    public string Name { get; set; } = null!;
    public string OwnerUserId { get; set; } = null!;
    public List<string> SharedUserIds { get; set; } = new();
    public List<TaskItemDto>? Tasks { get; set; }
}
