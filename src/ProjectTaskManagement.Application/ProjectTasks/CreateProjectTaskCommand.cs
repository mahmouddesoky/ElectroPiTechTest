using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common.Exceptions;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.Contracts;
using ProjectTaskManagement.Application.DTOs;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.Application.ProjectTasks;

public sealed record CreateProjectTaskCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    DateTime? DueDate,
    ProjectTaskPriority Priority) : IRequest<ApiResponse<ProjectTaskDto>>;

public sealed class CreateProjectTaskCommandValidator : AbstractValidator<CreateProjectTaskCommand>
{
    public CreateProjectTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Priority).IsInEnum();
    }
}

public sealed class CreateProjectTaskCommandHandler : IRequestHandler<CreateProjectTaskCommand, ApiResponse<ProjectTaskDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public CreateProjectTaskCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<ProjectTaskDto>> Handle(CreateProjectTaskCommand request, CancellationToken cancellationToken)
    {
        var projectExists = await _dbContext.Projects
            .AnyAsync(project => project.Id == request.ProjectId && project.UserId == _currentUser.UserId, cancellationToken);

        if (!projectExists)
        {
            throw new NotFoundException("Project", request.ProjectId);
        }

        var task = new ProjectTask
        {
            ProjectId = request.ProjectId,
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            DueDate = request.DueDate,
            Priority = request.Priority,
            Status = ProjectTaskStatus.Todo
        };

        _dbContext.ProjectTasks.Add(task);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ApiResponse<ProjectTaskDto>.Ok(task.ToDto(), "Task created successfully");
    }
}
