namespace ProjectTaskManagement.Application.DTOs;

public sealed record AuthResponse(
    string UserId,
    string Email,
    IReadOnlyCollection<string> Roles,
    string Token,
    DateTime ExpiresAt);
