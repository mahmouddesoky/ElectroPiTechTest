using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.DTOs;

namespace ProjectTaskManagement.Application.Contracts;

public interface IIdentityService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(string fullName, string email, string password, CancellationToken cancellationToken);
    Task<ApiResponse<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken);
}
