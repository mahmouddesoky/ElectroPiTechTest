using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common.Exceptions;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.Contracts;
using ProjectTaskManagement.Application.DTOs;

namespace ProjectTaskManagement.Application.Projects;

public sealed record GetProjectByIdQuery(Guid Id) : IRequest<ApiResponse<ProjectDto>>;

public sealed class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ApiResponse<ProjectDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public GetProjectByIdQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<ProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.Id == request.Id && p.UserId == _currentUser.UserId)
            .Select(p => p.ToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (project is null)
        {
            throw new NotFoundException("Project", request.Id);
        }

        return ApiResponse<ProjectDto>.Ok(project);
    }
}
