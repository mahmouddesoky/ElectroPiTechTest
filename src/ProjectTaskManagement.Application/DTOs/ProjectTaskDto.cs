using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.Application.DTOs;

public sealed record ProjectTaskDto(
    Guid Id,
    string Title,
    string? Description,
    ProjectTaskStatus Status,
    DateTime? DueDate,
    ProjectTaskPriority Priority,
    Guid ProjectId);
