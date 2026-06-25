using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Contracts;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Infrastructure.Identity;

namespace ProjectTaskManagement.Infrastructure.Persistence;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Project>(entity =>
        {
            entity.HasKey(project => project.Id);
            entity.Property(project => project.Name).HasMaxLength(150).IsRequired();
            entity.Property(project => project.Description).HasMaxLength(1000);
            entity.Property(project => project.CreatedAt).IsRequired();
            entity.Property(project => project.UserId).HasMaxLength(450).IsRequired();

            entity.HasIndex(project => project.UserId);

            entity.HasMany(project => project.Tasks)
                .WithOne(task => task.Project)
                .HasForeignKey(task => task.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(task => task.Id);
            entity.Property(task => task.Title).HasMaxLength(200).IsRequired();
            entity.Property(task => task.Description).HasMaxLength(1000);
            entity.Property(task => task.Status).HasConversion<int>().IsRequired();
            entity.Property(task => task.Priority).HasConversion<int>().IsRequired();

            entity.HasIndex(task => task.ProjectId);
            entity.HasIndex(task => new { task.ProjectId, task.Status });
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.FullName).HasMaxLength(150).IsRequired();
        });
    }
}
