using OrdersBackend.Application.Abstractions;

namespace OrdersBackend.Infrastructure.Authentication;

internal sealed class InMemoryUserAuthenticator : IUserAuthenticator
{
    private const string Email = "dev@martech.com";
    private const string Password = "Senha@123";

    public bool IsValid(string email, string password) =>
        string.Equals(email, Email, StringComparison.OrdinalIgnoreCase) && password == Password;
}
