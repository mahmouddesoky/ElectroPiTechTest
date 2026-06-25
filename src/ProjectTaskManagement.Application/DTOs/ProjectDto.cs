namespace ProjectTaskManagement.Application.DTOs;

public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt);
