using FluentValidation;
using MediatR;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.Contracts;
using ProjectTaskManagement.Application.DTOs;
using ProjectTaskManagement.Domain.Entities;

namespace ProjectTaskManagement.Application.Projects;

public sealed record CreateProjectCommand(string Name, string? Description)
    : IRequest<ApiResponse<ProjectDto>>;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}

public sealed class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ApiResponse<ProjectDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public CreateProjectCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<ProjectDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            UserId = _currentUser.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Projects.Add(project);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ApiResponse<ProjectDto>.Ok(project.ToDto(), "Project created successfully");
    }
}
