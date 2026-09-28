using NSubstitute;
using OrdersBackend.Application.Abstractions;
using OrdersBackend.Application.Auth.Commands.Login;
using OrdersBackend.Application.Common.Exceptions;

namespace OrdersBackend.Application.Tests.Auth.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly IUserAuthenticator _userAuthenticator = Substitute.For<IUserAuthenticator>();
    private readonly IJwtTokenGenerator _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_userAuthenticator, _jwtTokenGenerator);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsGeneratedToken()
    {
        var expiresAt = new DateTime(2026, 1, 15, 11, 30, 0, DateTimeKind.Utc);
        _userAuthenticator.IsValid("dev@martech.com", "Senha@123").Returns(true);
        _jwtTokenGenerator.Generate("dev@martech.com").Returns(new AccessToken("jwt-token", expiresAt));

        var response = await _handler.Handle(new LoginCommand("dev@martech.com", "Senha@123"), CancellationToken.None);

        Assert.Equal("jwt-token", response.AccessToken);
        Assert.Equal("Bearer", response.TokenType);
        Assert.Equal(expiresAt, response.ExpiresAt);
    }

    [Fact]
    public async Task Handle_WithInvalidCredentials_ThrowsInvalidCredentialsException()
    {
        _userAuthenticator.IsValid(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => _handler.Handle(new LoginCommand("dev@martech.com", "wrong"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithInvalidCredentials_DoesNotGenerateToken()
    {
        _userAuthenticator.IsValid(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => _handler.Handle(new LoginCommand("dev@martech.com", "wrong"), CancellationToken.None));

        _jwtTokenGenerator.DidNotReceive().Generate(Arg.Any<string>());
    }
}
