using Microsoft.AspNetCore.Mvc;
using TaskListApi.Dtos;
using TaskListApi.Mapping;
using TaskListApi.Models;
using TaskListApi.Repositories.Interfaces;
using TaskListApi.Services.Interfaces;

namespace TaskListApi.Services;

public class TaskListService : ITaskListService
{
    private readonly ITaskListRepository _repo;

    public TaskListService(ITaskListRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TaskListSimpleDto>> GetTaskListsAsync(string userId, int page, int pageSize)
    {
        var lists = await _repo.GetAllAsync(userId, page, pageSize);
        return lists.Select(l =>
        TaskListMapper.ToTaskListSimpleDto(l)
        ).ToList();
    }

    public async Task<TaskListDto?> GetByIdAsync(string taskListId)
    {
        if (string.IsNullOrWhiteSpace(taskListId))
            throw new ArgumentException("Id cannot be null or empty.", nameof(taskListId));

        var list = await _repo.GetByIdAsync(taskListId);
        return list != null ? TaskListMapper.ToDto(list) : null;
    }

    public async Task CreateAsync(CreateTaskListDto dto)
    {
        var newList = TaskListMapper.ToModel(dto);
        await _repo.CreateAsync(newList);
    }

    public async Task UpdateAsync(string taskListId, UpdateTaskListDto dto)
    {
        if (string.IsNullOrWhiteSpace(taskListId))
            throw new ArgumentException("Id cannot be null or empty.", nameof(taskListId));

        var list = await _repo.GetByIdAsync(taskListId);

        if (list == null || (list.OwnerUserId != dto.OwnerUserId && !list.SharedUserIds.Contains(dto.OwnerUserId)))
            throw new UnauthorizedAccessException();

        list.Name = dto.Name;
        list.SharedUserIds = dto.SharedUserIds;
        list.UpdatedAt = DateTime.UtcNow;

        if (dto.Tasks != null)
        {
            list.Tasks = dto.Tasks.Select(t => new TaskItem
            {
                Id = t.Id ?? Guid.NewGuid().ToString(),
                Title = t.Title,
                IsCompleted = t.IsCompleted,
                UpdatedAt = DateTime.UtcNow
            }).ToList();
        }

        await _repo.UpdateAsync(list);
    }

    public async Task DeleteAsync(string taskListId, string userId)
    {
        if (string.IsNullOrWhiteSpace(taskListId))
            throw new ArgumentException("Id cannot be null or empty.", nameof(taskListId));

        var list = await _repo.GetByIdAsync(taskListId);
        if (list == null || list.OwnerUserId != userId)
            throw new UnauthorizedAccessException();
        await _repo.DeleteAsync(taskListId);
    }

    public async Task<List<string>> GetSharedUsersAsync(string taskListId, string userId)
    {
        var list = await _repo.GetByIdAsync(taskListId);

        if (list == null || (userId !=null && list.OwnerUserId != userId && !list.SharedUserIds.Contains(userId)))
            throw new UnauthorizedAccessException();

        return list.SharedUserIds;
    }

    public async Task<IActionResult> ShareAsync(string taskListId, string ownerUserId, string sharedWithUserId)
    {
        if (string.IsNullOrWhiteSpace(ownerUserId))
            return new OkObjectResult($"{nameof(ownerUserId)} is required but was empty.");

        if (string.IsNullOrWhiteSpace(sharedWithUserId))
            return new OkObjectResult($"{nameof(sharedWithUserId)} is required but was empty.");

        var list = await _repo.GetByIdAsync(taskListId);
        if (list == null)
            return new OkObjectResult($"Task list with ID '{taskListId}' not found.");

        if (list.OwnerUserId != ownerUserId && !list.SharedUserIds.Contains(ownerUserId))
        {
            return new OkObjectResult($"To add new SharedUserIds ownerUserId '{ownerUserId}' should be the owner of the TaskList or already in SharedUserIds list. No action taken.");
        }

        if (!list.SharedUserIds.Contains(sharedWithUserId))
        {
            list.SharedUserIds.Add(sharedWithUserId);
            await _repo.UpdateAsync(list);
            return new OkObjectResult($"User '{sharedWithUserId}' has been added to SharedUserIds.");
        }

        return new OkObjectResult($"User '{sharedWithUserId}' was already present in SharedUserIds.");
    }

    public async Task<IActionResult> UnshareAsync(string taskListId, string ownerUserId, string sharedWithUserId)
    {
        if (string.IsNullOrWhiteSpace(ownerUserId))
            return new OkObjectResult($"{nameof(ownerUserId)} is required but was empty.");

        if (string.IsNullOrWhiteSpace(sharedWithUserId))
            return new OkObjectResult($"{nameof(sharedWithUserId)} is required but was empty.");

        var list = await _repo.GetByIdAsync(taskListId);
        if (list == null)
            return new OkObjectResult($"Task list with ID '{taskListId}' not found.");

        if (list.OwnerUserId != ownerUserId && !list.SharedUserIds.Contains(ownerUserId))
        {
            return new OkObjectResult($"To remove existing SharedUser ownerUserId '{ownerUserId}' should be the owner of the TaskList or in SharedUserIds list. No action taken.");
        }

        if (!list.SharedUserIds.Contains(sharedWithUserId))
        {
            return new OkObjectResult($"User '{sharedWithUserId}' is not in SharedUserIds. Nothing to remove.");
        }

        list.SharedUserIds.Remove(sharedWithUserId);
        await _repo.UpdateAsync(list);

        return new OkObjectResult($"User '{sharedWithUserId}' has been successfully removed from SharedUserIds.");
    }
}

