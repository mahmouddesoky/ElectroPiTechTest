using FluentValidation;
using MediatR;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.Contracts;
using ProjectTaskManagement.Application.DTOs;

namespace ProjectTaskManagement.Application.Auth;

public sealed record LoginCommand(string Email, string Password)
    : IRequest<ApiResponse<AuthResponse>>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty();
    }
}

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponse>>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<ApiResponse<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return _identityService.LoginAsync(request.Email, request.Password, cancellationToken);
    }
}
