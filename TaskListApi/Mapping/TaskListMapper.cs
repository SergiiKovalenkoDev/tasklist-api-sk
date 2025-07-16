using System.Collections.Generic;
using TaskListApi.Dtos;
using TaskListApi.Models;

namespace TaskListApi.Mapping;

public static class TaskListMapper
{
    public static TaskListDto ToDto(TaskList model)
    {
        return new TaskListDto
        {
            Id = model.Id,
            Name = model.Name,
            OwnerUserId = model.OwnerUserId,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
            SharedUserIds = model.SharedUserIds,

            Tasks = model.Tasks.Select(t =>
            TaskItemMapper.ToDto(t)).ToList()
        };
    }

    public static TaskList ToModel(CreateTaskListDto dto)
    {
        return new TaskList
        {
            Name = dto.Name,
            OwnerUserId = dto.OwnerUserId,
            SharedUserIds = dto.SharedUserIds,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Tasks = dto.Tasks?.Select(t => new TaskItem
            {
                Title = t.Title,
                IsCompleted = t.IsCompleted,
                CreatedAt = t.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            }).ToList() ?? new()
        };
    }

    public static void UpdateModel(TaskList model, UpdateTaskListDto dto)
    {
        model.Name = dto.Name;
        // ...
    }

    public static TaskListSimpleDto ToTaskListSimpleDto(TaskList model)
    {
        return new TaskListSimpleDto
        {
            Id = model.Id,
            Name = model.Name,
            SharedUsersAmount = model.SharedUserIds.Count,
            TasksAmount = model.Tasks.Count
        };
    }
}
