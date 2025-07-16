namespace TaskListApi.Dtos
{
    public class TaskListSimpleDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int SharedUsersAmount { get; set; } = 0;
        public int TasksAmount { get; set; } = 0;

    }
}
