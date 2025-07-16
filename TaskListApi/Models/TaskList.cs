using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskListApi.Models;

public class TaskList
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string OwnerUserId { get; set; } = null!;
    public List<string> SharedUserIds { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<TaskItem> Tasks { get; set; } = new();
}
