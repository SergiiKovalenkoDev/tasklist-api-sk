using Microsoft.Extensions.Logging;
using Moq;
using TaskListApi.Dtos;
using TaskListApi.Models;
using TaskListApi.Repositories.Interfaces;
using TaskListApi.Services;
using TaskListApi.Services.Interfaces;

namespace TaskListApi.Tests.Services;

public class TaskListServiceTests
{
    private readonly Mock<ITaskListRepository> _repoMock;
    private readonly ITaskListService _service;

    public TaskListServiceTests()
    {
        var repoMock = new Mock<ITaskListRepository>();
        var loggerMock = new Mock<ILogger<TaskListService>>();

        var service = new TaskListService(repoMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_Call_Repo_With_Correct_Data()
    {
        // Arrange
        var dto = new CreateTaskListDto
        {
            Name = "My List",
            OwnerUserId = "user123"
        };

        // Act
        await _service.CreateAsync(dto);

        // Assert
        _repoMock.Verify(r => r.CreateAsync(It.Is<TaskList>(l =>
            l.Name == dto.Name &&
            l.OwnerUserId == dto.OwnerUserId &&
            l.SharedUserIds.Count == 0
        )), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_If_Not_Owner()
    {
        // Arrange
        var id = "list1";
        var userId = "user123";

        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new TaskList
        {
            Id = id,
            Name = "Test",
            OwnerUserId = "anotherUser"
        });

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.DeleteAsync(id, userId));
    }

    [Fact]
    public async Task GetUserTaskListsAsync_Should_Return_Expected_Dtos()
    {
        // Arrange
        var userId = "user123";
        _repoMock.Setup(r => r.GetAllAsync(userId, 1, 10)).ReturnsAsync(new List<TaskList>
        {
            new TaskList
            {
                Id = "1",
                Name = "List 1",
                OwnerUserId = userId,
                CreatedAt = DateTime.UtcNow
            }
        });

        // Act
        var result = await _service.GetTaskListsAsync(userId, 1, 10);

        // Assert
        Assert.Single(result);
        Assert.Equal("1", result[0].Id);
        Assert.Equal("List 1", result[0].Name);
    }
}
