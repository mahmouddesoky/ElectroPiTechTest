using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common.Exceptions;
using ProjectTaskManagement.Application.Contracts;
using ProjectTaskManagement.Application.ProjectTasks;
using ProjectTaskManagement.Application.Projects;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Enums;
using ProjectTaskManagement.Infrastructure.Persistence;

namespace ProjectTaskManagement.Tests;

public sealed class ApplicationHandlerTests
{
    [Fact]
    public async Task CreateProject_Should_Create_Project_For_Current_User()
    {
        await using var dbContext = CreateDbContext();
        var currentUser = new TestCurrentUserService("user-1");
        var handler = new CreateProjectCommandHandler(dbContext, currentUser);

        var response = await handler.Handle(new CreateProjectCommand("CRM Rewrite", "Internal project"), CancellationToken.None);

        response.Success.Should().BeTrue();
        response.Data!.Name.Should().Be("CRM Rewrite");
        dbContext.Projects.Single().UserId.Should().Be("user-1");
    }

    [Fact]
    public async Task GetProjects_Should_Return_Only_Current_User_Projects()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Projects.AddRange(
            new Project { Name = "Mine", UserId = "user-1" },
            new Project { Name = "Other", UserId = "user-2" });
        await dbContext.SaveChangesAsync();

        var handler = new GetProjectsQueryHandler(dbContext, new TestCurrentUserService("user-1"));
        var response = await handler.Handle(new GetProjectsQuery(), CancellationToken.None);

        response.Data.Should().ContainSingle();
        response.Data!.Single().Name.Should().Be("Mine");
    }

    [Fact]
    public async Task UpdateTaskStatus_Should_Not_Update_Task_From_Another_User_Project()
    {
        await using var dbContext = CreateDbContext();
        var project = new Project { Name = "Other project", UserId = "user-2" };
        var task = new ProjectTask { Title = "Private task", Project = project, Priority = ProjectTaskPriority.High };
        dbContext.ProjectTasks.Add(task);
        await dbContext.SaveChangesAsync();

        var handler = new UpdateProjectTaskStatusCommandHandler(dbContext, new TestCurrentUserService("user-1"));

        var act = () => handler.Handle(new UpdateProjectTaskStatusCommand(task.Id, ProjectTaskStatus.Done), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public void CreateProjectValidator_Should_Reject_Empty_Name()
    {
        var validator = new CreateProjectCommandValidator();

        var result = validator.Validate(new CreateProjectCommand("", "Description"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "Name");
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private sealed class TestCurrentUserService : ICurrentUserService
    {
        public TestCurrentUserService(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
        public bool IsAuthenticated => true;
    }
}
