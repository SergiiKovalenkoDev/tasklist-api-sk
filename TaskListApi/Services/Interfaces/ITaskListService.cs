using Microsoft.AspNetCore.Mvc;
using TaskListApi.Dtos;

namespace TaskListApi.Services.Interfaces;

public interface ITaskListService
{
    Task<List<TaskListSimpleDto>> GetTaskListsAsync(string userId, int page, int pageSize);
    Task<TaskListDto?> GetByIdAsync(string id);
    Task CreateAsync(CreateTaskListDto dto);
    Task UpdateAsync(string id, UpdateTaskListDto dto);
    Task DeleteAsync(string id, string userId);
    Task<List<string>> GetSharedUsersAsync(string id, string userId);
    Task<IActionResult> ShareAsync(string setId, string ownerUserId, string sharedWithUserId);
    Task<IActionResult> UnshareAsync(string setId, string ownerUserId, string sharedWithUserId);
}
