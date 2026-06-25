using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.DTOs;
using ProjectTaskManagement.Application.Projects;

namespace ProjectTaskManagement.Api.Controllers;

[Authorize(Roles = "User,Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/projects")]
public sealed class ProjectsController : ControllerBase
{
    private readonly ISender _sender;

    public ProjectsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProjectDto>>> Create(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(command, cancellationToken));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ProjectDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ProjectDto>>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetProjectsQuery(), cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProjectDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetProjectByIdQuery(id), cancellationToken));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProjectDto>>> Update(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new UpdateProjectCommand(id, request.Name, request.Description), cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new DeleteProjectCommand(id), cancellationToken));
    }
}

public sealed record UpdateProjectRequest(string Name, string? Description);
