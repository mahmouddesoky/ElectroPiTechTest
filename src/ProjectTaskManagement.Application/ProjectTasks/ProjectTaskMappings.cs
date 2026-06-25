using ProjectTaskManagement.Application.DTOs;
using ProjectTaskManagement.Domain.Entities;

namespace ProjectTaskManagement.Application.ProjectTasks;

internal static class ProjectTaskMappings
{
    public static ProjectTaskDto ToDto(this ProjectTask task)
    {
        return new ProjectTaskDto(
            task.Id,
            task.Title,
            task.Description,
            task.Status,
            task.DueDate,
            task.Priority,
            task.ProjectId);
    }
}
