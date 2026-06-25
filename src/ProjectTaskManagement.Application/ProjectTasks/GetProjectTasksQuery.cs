using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common.Exceptions;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.Contracts;
using ProjectTaskManagement.Application.DTOs;

namespace ProjectTaskManagement.Application.ProjectTasks;

public sealed record GetProjectTasksQuery(Guid ProjectId)
    : IRequest<ApiResponse<IReadOnlyCollection<ProjectTaskDto>>>;

public sealed class GetProjectTasksQueryHandler : IRequestHandler<GetProjectTasksQuery, ApiResponse<IReadOnlyCollection<ProjectTaskDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public GetProjectTasksQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<IReadOnlyCollection<ProjectTaskDto>>> Handle(GetProjectTasksQuery request, CancellationToken cancellationToken)
    {
        var projectExists = await _dbContext.Projects
            .AnyAsync(project => project.Id == request.ProjectId && project.UserId == _currentUser.UserId, cancellationToken);

        if (!projectExists)
        {
            throw new NotFoundException("Project", request.ProjectId);
        }

        var tasks = await _dbContext.ProjectTasks
            .AsNoTracking()
            .Where(task => task.ProjectId == request.ProjectId)
            .OrderBy(task => task.DueDate ?? DateTime.MaxValue)
            .ThenByDescending(task => task.Priority)
            .Select(task => task.ToDto())
            .ToArrayAsync(cancellationToken);

        return ApiResponse<IReadOnlyCollection<ProjectTaskDto>>.Ok(tasks);
    }
}
