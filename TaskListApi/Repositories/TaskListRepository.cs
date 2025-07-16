using MongoDB.Driver;
using TaskListApi.Database;
using TaskListApi.Models;
using TaskListApi.Repositories.Interfaces;

namespace TaskListApi.Repositories;

public class TaskListRepository : ITaskListRepository
{
    private readonly IMongoCollection<TaskList> _collection;

    public TaskListRepository(MongoDbContext context)
    {
        _collection = context.TaskLists;
    }

    public Task<List<TaskList>> GetAllAsync(string userId, int page, int pageSize)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 10;

        //var filter = Builders<TaskList>.Filter.Or(
        //        Builders<TaskList>.Filter.Eq(l => l.OwnerUserId, userId),
        //        Builders<TaskList>.Filter.AnyEq(l => l.SharedUserIds, userId)
        //    );

        return _collection.Find(l => l.OwnerUserId == userId || l.SharedUserIds.Contains(userId))
                   .SortByDescending(l => l.CreatedAt)
                   .Skip((page - 1) * pageSize)
                   .Limit(pageSize)
                   .ToListAsync();
    }

    public Task<TaskList?> GetByIdAsync(string id) =>
        _collection.Find(l => l.Id == id).FirstOrDefaultAsync();

    public Task CreateAsync(TaskList list) =>
        _collection.InsertOneAsync(list);

    public Task UpdateAsync(TaskList list) =>
        _collection.ReplaceOneAsync(l => l.Id == list.Id, list);

    public Task DeleteAsync(string id) =>
        _collection.DeleteOneAsync(l => l.Id == id);
}

