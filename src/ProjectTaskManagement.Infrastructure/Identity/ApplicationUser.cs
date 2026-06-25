using Microsoft.AspNetCore.Identity;

namespace ProjectTaskManagement.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
