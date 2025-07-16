namespace TaskListApi.Dtos;

public class ShareTaskListDto
{
    public string OwnerUserId { get; set; } = null!;
    public string SharedWithUserId { get; set; } = null!;
}
