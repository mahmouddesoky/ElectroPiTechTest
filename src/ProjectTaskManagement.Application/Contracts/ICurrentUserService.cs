namespace ProjectTaskManagement.Application.Contracts;

public interface ICurrentUserService
{
    string UserId { get; }
    bool IsAuthenticated { get; }
}
