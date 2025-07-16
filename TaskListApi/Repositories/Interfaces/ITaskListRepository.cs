using TaskListApi.Models;

namespace TaskListApi.Repositories.Interfaces;

public interface ITaskListRepository
{
    Task<List<TaskList>> GetAllAsync(string id, int page, int pageSize);
    Task<TaskList?> GetByIdAsync(string id);
    Task CreateAsync(TaskList list);
    Task UpdateAsync(TaskList list);
    Task DeleteAsync(string id);
}

