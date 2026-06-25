using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common.Exceptions;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.Contracts;
using ProjectTaskManagement.Application.DTOs;
using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.Application.ProjectTasks;

public sealed record UpdateProjectTaskStatusCommand(Guid Id, ProjectTaskStatus Status)
    : IRequest<ApiResponse<ProjectTaskDto>>;

public sealed class UpdateProjectTaskStatusCommandValidator : AbstractValidator<UpdateProjectTaskStatusCommand>
{
    public UpdateProjectTaskStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

public sealed class UpdateProjectTaskStatusCommandHandler : IRequestHandler<UpdateProjectTaskStatusCommand, ApiResponse<ProjectTaskDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public UpdateProjectTaskStatusCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<ProjectTaskDto>> Handle(UpdateProjectTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await _dbContext.ProjectTasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.Project.UserId == _currentUser.UserId, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task", request.Id);
        }

        task.Status = request.Status;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ApiResponse<ProjectTaskDto>.Ok(task.ToDto(), "Task status updated successfully");
    }
}
