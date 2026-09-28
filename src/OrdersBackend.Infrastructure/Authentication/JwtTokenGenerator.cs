using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OrdersBackend.Application.Abstractions;

namespace OrdersBackend.Infrastructure.Authentication;

internal sealed class JwtTokenGenerator(IOptions<JwtOptions> options, TimeProvider timeProvider) : IJwtTokenGenerator
{
    private readonly JwtOptions _options = options.Value;
    private readonly JsonWebTokenHandler _tokenHandler = new();

    public AccessToken Generate(string email)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = now.AddMinutes(_options.ExpirationMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ]),
            IssuedAt = now,
            NotBefore = now,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(_options.CreateSigningKey(), SecurityAlgorithms.HmacSha256)
        };

        return new AccessToken(_tokenHandler.CreateToken(descriptor), expiresAt);
    }
}
