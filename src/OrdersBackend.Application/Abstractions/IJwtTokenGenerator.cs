namespace OrdersBackend.Application.Abstractions;

public interface IJwtTokenGenerator
{
    AccessToken Generate(string email);
}

public sealed record AccessToken(string Token, DateTime ExpiresAt);
