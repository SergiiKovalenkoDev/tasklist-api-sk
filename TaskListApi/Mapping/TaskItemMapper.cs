using TaskListApi.Dtos;
using TaskListApi.Models;

namespace TaskListApi.Mapping;

public class TaskItemMapper
{

    public static TaskItemDto ToDto(TaskItem model)
    {
        return new TaskItemDto
        {
            Id = model.Id,
            Title = model.Title,
            IsCompleted = model.IsCompleted,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }



    public static void UpdateModel(TaskItem model, TaskItemDto dto)
    {
        //model.Name = dto.Name;
        // ...
    }
}
