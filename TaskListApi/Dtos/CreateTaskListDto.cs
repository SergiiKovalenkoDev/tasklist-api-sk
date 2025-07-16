using System.ComponentModel.DataAnnotations;

namespace TaskListApi.Dtos;

public class CreateTaskListDto
{
    [Required]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 255 characters.")]
    public string Name { get; set; } = null!;
    public string OwnerUserId { get; set; } = null!;

    public List<string> SharedUserIds { get; set; } = new();
    public DateTime CreatedAt { get; set; }

    public List<TaskItemDto>? Tasks { get; set; }
}
