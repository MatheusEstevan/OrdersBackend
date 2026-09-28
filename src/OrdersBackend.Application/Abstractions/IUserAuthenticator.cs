namespace OrdersBackend.Application.Abstractions;

public interface IUserAuthenticator
{
    bool IsValid(string email, string password);
}
