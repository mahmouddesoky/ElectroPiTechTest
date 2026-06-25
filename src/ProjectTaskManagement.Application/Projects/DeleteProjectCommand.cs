using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common.Exceptions;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.Contracts;

namespace ProjectTaskManagement.Application.Projects;

public sealed record DeleteProjectCommand(Guid Id) : IRequest<ApiResponse<object>>;

public sealed class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, ApiResponse<object>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public DeleteProjectCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<object>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.UserId == _currentUser.UserId, cancellationToken);

        if (project is null)
        {
            throw new NotFoundException("Project", request.Id);
        }

        _dbContext.Projects.Remove(project);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ApiResponse<object>.Ok(new { project.Id }, "Project deleted successfully");
    }
}
