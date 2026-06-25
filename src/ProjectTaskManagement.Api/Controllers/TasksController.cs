using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.DTOs;
using ProjectTaskManagement.Application.ProjectTasks;
using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.Api.Controllers;

[Authorize(Roles = "User,Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
public sealed class TasksController : ControllerBase
{
    private readonly ISender _sender;

    public TasksController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("projects/{projectId:guid}/tasks")]
    [ProducesResponseType(typeof(ApiResponse<ProjectTaskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProjectTaskDto>>> Create(
        Guid projectId,
        CreateProjectTaskRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProjectTaskCommand(projectId, request.Title, request.Description, request.DueDate, request.Priority);
        return Ok(await _sender.Send(command, cancellationToken));
    }

    [HttpGet("projects/{projectId:guid}/tasks")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ProjectTaskDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ProjectTaskDto>>>> GetByProject(Guid projectId, CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetProjectTasksQuery(projectId), cancellationToken));
    }

    [HttpPatch("tasks/{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<ProjectTaskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProjectTaskDto>>> UpdateStatus(
        Guid id,
        UpdateProjectTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new UpdateProjectTaskStatusCommand(id, request.Status), cancellationToken));
    }

    [HttpDelete("tasks/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new DeleteProjectTaskCommand(id), cancellationToken));
    }
}

public sealed record CreateProjectTaskRequest(
    string Title,
    string? Description,
    DateTime? DueDate,
    ProjectTaskPriority Priority);

public sealed record UpdateProjectTaskStatusRequest(ProjectTaskStatus Status);
