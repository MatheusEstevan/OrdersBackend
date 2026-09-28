using MediatR;
using OrdersBackend.Application.Abstractions;
using OrdersBackend.Application.Common.Exceptions;

namespace OrdersBackend.Application.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserAuthenticator _userAuthenticator;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(IUserAuthenticator userAuthenticator, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userAuthenticator = userAuthenticator;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (!_userAuthenticator.IsValid(request.Email, request.Password))
            throw new InvalidCredentialsException();

        var accessToken = _jwtTokenGenerator.Generate(request.Email);

        return Task.FromResult(new LoginResponse(accessToken.Token, "Bearer", accessToken.ExpiresAt));
    }
}
