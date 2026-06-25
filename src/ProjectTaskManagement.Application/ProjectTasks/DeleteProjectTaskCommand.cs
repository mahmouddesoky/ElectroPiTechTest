using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common.Exceptions;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.Contracts;

namespace ProjectTaskManagement.Application.ProjectTasks;

public sealed record DeleteProjectTaskCommand(Guid Id) : IRequest<ApiResponse<object>>;

public sealed class DeleteProjectTaskCommandHandler : IRequestHandler<DeleteProjectTaskCommand, ApiResponse<object>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public DeleteProjectTaskCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<object>> Handle(DeleteProjectTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _dbContext.ProjectTasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.Project.UserId == _currentUser.UserId, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task", request.Id);
        }

        _dbContext.ProjectTasks.Remove(task);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ApiResponse<object>.Ok(new { task.Id }, "Task deleted successfully");
    }
}
