using MediatR;

namespace OrdersBackend.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

public sealed record LoginResponse(string AccessToken, string TokenType, DateTime ExpiresAt);
