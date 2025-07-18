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
    private readonly ILogger<TaskListService> _logger;

    public TaskListService(ITaskListRepository repo, ILogger<TaskListService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<TaskListSimpleDto>> GetTaskListsAsync(string userId, int page, int pageSize)
    {
        _logger.LogInformation("Getting task lists for user {UserId}, page {Page}, pageSize {PageSize}", userId, page, pageSize);
        var lists = await _repo.GetAllAsync(userId, page, pageSize);
        return lists.Select(l =>
        TaskListMapper.ToTaskListSimpleDto(l)
        ).ToList();
    }

    public async Task<TaskListDto?> GetByIdAsync(string taskListId)
    {
        if (string.IsNullOrWhiteSpace(taskListId))
        {
            _logger.LogWarning("GetByIdAsync called with empty taskListId");
            throw new ArgumentException("Id cannot be null or empty.", nameof(taskListId));
        }

        var list = await _repo.GetByIdAsync(taskListId);
        if (list == null)
        {
            _logger.LogWarning("Task list with ID {TaskListId} not found", taskListId);
        }
        return list != null ? TaskListMapper.ToDto(list) : null;
    }

    public async Task<TaskListDto> CreateAsync(CreateTaskListDto dto)
    {
        if (dto == null)
        {
            _logger.LogWarning("CreateAsync called with null dto");
            throw new ArgumentNullException(nameof(dto));
        }

        // Додаткова перевірка, якщо потрібно
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            _logger.LogWarning("CreateAsync called with empty Name");
            throw new ArgumentException("Name is required", nameof(dto.Name));
        }

        if (string.IsNullOrWhiteSpace(dto.OwnerUserId))
        {
            _logger.LogWarning("CreateAsync called with empty OwnerUserId");
            throw new ArgumentException("OwnerUserId is required", nameof(dto.OwnerUserId));
        }

        _logger.LogInformation("Creating new task list for owner {OwnerUserId} with name '{Name}'", dto.OwnerUserId, dto.Name);

        var newList = TaskListMapper.ToModel(dto);
        await _repo.CreateAsync(newList);

        _logger.LogInformation("Task list created with id {TaskListId}", newList.Id);

        // Повертаємо DTO для контролера
        return TaskListMapper.ToDto(newList);
    }

    public async Task UpdateAsync(string taskListId, UpdateTaskListDto dto)
    {
        if (string.IsNullOrWhiteSpace(taskListId))
        {
            _logger.LogWarning("UpdateAsync called with empty taskListId");
            throw new ArgumentException("Id cannot be null or empty.", nameof(taskListId));
        }

        var list = await _repo.GetByIdAsync(taskListId);

        if (list == null || (list.OwnerUserId != dto.OwnerUserId && !list.SharedUserIds.Contains(dto.OwnerUserId)))
        {
            _logger.LogWarning("Unauthorized update attempt for taskListId {TaskListId} by user {UserId}", taskListId, dto.OwnerUserId);
            throw new UnauthorizedAccessException();
        }

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
        _logger.LogInformation("Task list {TaskListId} updated by user {UserId}", taskListId, dto.OwnerUserId);
    }

    public async Task DeleteAsync(string taskListId, string userId)
    {
        if (string.IsNullOrWhiteSpace(taskListId))
        {
            _logger.LogWarning("DeleteAsync called with empty taskListId");
            throw new ArgumentException("Id cannot be null or empty.", nameof(taskListId));
        }

        var list = await _repo.GetByIdAsync(taskListId);
        if (list == null || list.OwnerUserId != userId)
        {
            _logger.LogWarning("Unauthorized delete attempt for taskListId {TaskListId} by user {UserId}", taskListId, userId);
            throw new UnauthorizedAccessException();
        }
        await _repo.DeleteAsync(taskListId);
        _logger.LogInformation("Task list {TaskListId} deleted by user {UserId}", taskListId, userId);
    }

    public async Task<List<string>> GetSharedUsersAsync(string taskListId, string userId)
    {
        var list = await _repo.GetByIdAsync(taskListId);

        if (list == null || (userId !=null && list.OwnerUserId != userId && !list.SharedUserIds.Contains(userId)))
        {
            _logger.LogWarning("Unauthorized access to shared users for taskListId {TaskListId} by user {UserId}", taskListId, userId);
            throw new UnauthorizedAccessException();
        }

        _logger.LogInformation("Getting shared users for taskListId {TaskListId}", taskListId);
        return list.SharedUserIds;
    }

    public async Task<IActionResult> ShareAsync(string taskListId, string ownerUserId, string sharedWithUserId)
    {
        if (string.IsNullOrWhiteSpace(ownerUserId))
        {
            _logger.LogWarning("ShareAsync called with empty ownerUserId");
            return new OkObjectResult($"{nameof(ownerUserId)} is required but was empty.");
        }

        if (string.IsNullOrWhiteSpace(sharedWithUserId))
        {
            _logger.LogWarning("ShareAsync called with empty sharedWithUserId");
            return new OkObjectResult($"{nameof(sharedWithUserId)} is required but was empty.");
        }

        var list = await _repo.GetByIdAsync(taskListId);
        if (list == null)
        {
            _logger.LogWarning("ShareAsync: Task list with ID {TaskListId} not found", taskListId);
            return new OkObjectResult($"Task list with ID '{taskListId}' not found.");
        }

        if (list.OwnerUserId != ownerUserId && !list.SharedUserIds.Contains(ownerUserId))
        {
            _logger.LogWarning("ShareAsync: Unauthorized share attempt for taskListId {TaskListId} by user {UserId}", taskListId, ownerUserId);
            return new OkObjectResult($"To add new SharedUserIds ownerUserId '{ownerUserId}' should be the owner of the TaskList or already in SharedUserIds list. No action taken.");
        }

        if (!list.SharedUserIds.Contains(sharedWithUserId))
        {
            list.SharedUserIds.Add(sharedWithUserId);
            await _repo.UpdateAsync(list);
            _logger.LogInformation("User {SharedWithUserId} added to SharedUserIds for taskListId {TaskListId}", sharedWithUserId, taskListId);
            return new OkObjectResult($"User '{sharedWithUserId}' has been added to SharedUserIds.");
        }

        _logger.LogInformation("User {SharedWithUserId} was already present in SharedUserIds for taskListId {TaskListId}", sharedWithUserId, taskListId);
        return new OkObjectResult($"User '{sharedWithUserId}' was already present in SharedUserIds.");
    }

    public async Task<IActionResult> UnshareAsync(string taskListId, string ownerUserId, string sharedWithUserId)
    {
        if (string.IsNullOrWhiteSpace(ownerUserId))
        {
            _logger.LogWarning("UnshareAsync called with empty ownerUserId");
            return new OkObjectResult($"{nameof(ownerUserId)} is required but was empty.");
        }

        if (string.IsNullOrWhiteSpace(sharedWithUserId))
        {
            _logger.LogWarning("UnshareAsync called with empty sharedWithUserId");
            return new OkObjectResult($"{nameof(sharedWithUserId)} is required but was empty.");
        }

        var list = await _repo.GetByIdAsync(taskListId);
        if (list == null)
        {
            _logger.LogWarning("UnshareAsync: Task list with ID {TaskListId} not found", taskListId);
            return new OkObjectResult($"Task list with ID '{taskListId}' not found.");
        }

        if (list.OwnerUserId != ownerUserId && !list.SharedUserIds.Contains(ownerUserId))
        {
            _logger.LogWarning("UnshareAsync: Unauthorized unshare attempt for taskListId {TaskListId} by user {UserId}", taskListId, ownerUserId);
            return new OkObjectResult($"To remove existing SharedUser ownerUserId '{ownerUserId}' should be the owner of the TaskList or in SharedUserIds list. No action taken.");
        }

        if (!list.SharedUserIds.Contains(sharedWithUserId))
        {
            _logger.LogInformation("User {SharedWithUserId} is not in SharedUserIds for taskListId {TaskListId}", sharedWithUserId, taskListId);
            return new OkObjectResult($"User '{sharedWithUserId}' is not in SharedUserIds. Nothing to remove.");
        }

        list.SharedUserIds.Remove(sharedWithUserId);
        await _repo.UpdateAsync(list);
        _logger.LogInformation("User {SharedWithUserId} removed from SharedUserIds for taskListId {TaskListId}", sharedWithUserId, taskListId);
        return new OkObjectResult($"User '{sharedWithUserId}' has been successfully removed from SharedUserIds.");
    }
}

