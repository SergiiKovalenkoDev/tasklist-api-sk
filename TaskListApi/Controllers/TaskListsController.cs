using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TaskListApi.Dtos;
using TaskListApi.Services.Interfaces;

namespace TaskListApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskListsController : ControllerBase
{
    private readonly ITaskListService _service;

    public TaskListsController(ITaskListService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var list = await _service.GetTaskListsAsync(userId, page, pageSize);
        return list.Count < 1 ? NotFound() : Ok(list);
    }

    [HttpGet("{taskListId}")]
    public async Task<IActionResult> Get(string taskListId)
    {
        var list = await _service.GetByIdAsync(taskListId);
        return list == null ? NotFound() : Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskListDto dto, [FromServices] FluentValidation.IValidator<CreateTaskListDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return BadRequest(ModelState);
        }

        await _service.CreateAsync(dto);
        return Ok();
    }

    [HttpPut("{taskListId}")]
    public async Task<IActionResult> Update(string taskListId, [FromBody] UpdateTaskListDto dto)
    {
        await _service.UpdateAsync(taskListId, dto);
        return Ok();
    }

    [HttpDelete("{taskListId}")]
    public async Task<IActionResult> Delete(string taskListId, [FromQuery] string ownerUserId)
    {
        await _service.DeleteAsync(taskListId, ownerUserId);
        return Ok();
    }

    [HttpGet("{taskListId}/shared-users")]
    public async Task<IActionResult> GetSharedUsers(string taskListId, [FromQuery] string userId)
    {
        var users = await _service.GetSharedUsersAsync(taskListId, userId);
        return Ok(users);
    }

    [HttpPost("{taskListId}/share")]
    public async Task<IActionResult> Share(string taskListId, [FromQuery] string ownerUserId, [FromQuery] string sharedWithUserId)
    {
        return await _service.ShareAsync(taskListId, ownerUserId, sharedWithUserId);
    }

    [HttpPost("{taskListId}/unshare")]
    public async Task<IActionResult> Unshare(string taskListId, [FromQuery] string ownerUserId, [FromQuery] string sharedWithUserId)
    {
        return await _service.UnshareAsync(taskListId, ownerUserId, sharedWithUserId);
    }

    [HttpGet("error")]
    public IActionResult GetError()
    {
        throw new Exception("Test error from controller!");
    }
}

