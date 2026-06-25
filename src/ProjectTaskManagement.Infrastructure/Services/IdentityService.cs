using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.Contracts;
using ProjectTaskManagement.Application.DTOs;
using ProjectTaskManagement.Infrastructure.Identity;

namespace ProjectTaskManagement.Infrastructure.Services;

internal sealed class IdentityService : IIdentityService
{
    private const string UserRole = "User";
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtTokenService _jwtTokenService;

    public IdentityService(UserManager<ApplicationUser> userManager, JwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(string fullName, string email, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToUpperInvariant();
        var emailExists = await _userManager.Users.AnyAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);
        if (emailExists)
        {
            return ApiResponse<AuthResponse>.Fail("Registration failed", new[] { "Email is already registered." });
        }

        var user = new ApplicationUser
        {
            FullName = fullName.Trim(),
            UserName = email.Trim(),
            Email = email.Trim(),
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return ApiResponse<AuthResponse>.Fail("Registration failed", result.Errors.Select(error => error.Description));
        }

        await _userManager.AddToRoleAsync(user, UserRole);
        var token = await _jwtTokenService.CreateTokenAsync(user, cancellationToken);
        return ApiResponse<AuthResponse>.Ok(token, "Registration completed successfully");
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToUpperInvariant();
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        if (user is null)
        {
            return ApiResponse<AuthResponse>.Fail("Login failed", new[] { "Invalid email or password." });
        }

        var passwordIsValid = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordIsValid)
        {
            return ApiResponse<AuthResponse>.Fail("Login failed", new[] { "Invalid email or password." });
        }

        var token = await _jwtTokenService.CreateTokenAsync(user, cancellationToken);
        return ApiResponse<AuthResponse>.Ok(token, "Login completed successfully");
    }
}
