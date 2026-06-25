using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.Domain.Entities;

public sealed class ProjectTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.Todo;
    public DateTime? DueDate { get; set; }
    public ProjectTaskPriority Priority { get; set; } = ProjectTaskPriority.Medium;
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
