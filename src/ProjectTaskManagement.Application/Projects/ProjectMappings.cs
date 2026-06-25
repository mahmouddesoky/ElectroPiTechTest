using ProjectTaskManagement.Application.DTOs;
using ProjectTaskManagement.Domain.Entities;

namespace ProjectTaskManagement.Application.Projects;

internal static class ProjectMappings
{
    public static ProjectDto ToDto(this Project project)
    {
        return new ProjectDto(project.Id, project.Name, project.Description, project.CreatedAt);
    }
}
