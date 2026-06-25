using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.Contracts;
using ProjectTaskManagement.Application.DTOs;

namespace ProjectTaskManagement.Application.Projects;

public sealed record GetProjectsQuery : IRequest<ApiResponse<IReadOnlyCollection<ProjectDto>>>;

public sealed class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, ApiResponse<IReadOnlyCollection<ProjectDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public GetProjectsQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<IReadOnlyCollection<ProjectDto>>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _dbContext.Projects
            .AsNoTracking()
            .Where(project => project.UserId == _currentUser.UserId)
            .OrderByDescending(project => project.CreatedAt)
            .Select(project => project.ToDto())
            .ToArrayAsync(cancellationToken);

        return ApiResponse<IReadOnlyCollection<ProjectDto>>.Ok(projects);
    }
}
