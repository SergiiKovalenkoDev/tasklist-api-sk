using MongoDB.Driver;
using TaskListApi.Models;

namespace TaskListApi.Database;

public class MongoDbContext
{
    private readonly IMongoDatabase _db;
    public IMongoCollection<TaskList> TaskLists => _db.GetCollection<TaskList>("TaskLists");

    public MongoDbContext(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDb:ConnectionString"]);
        _db = client.GetDatabase(config["MongoDb:Database"]);
    }
}

