using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Domain.Entities;

namespace ProjectTaskManagement.Application.Contracts;

public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<ProjectTask> ProjectTasks { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
