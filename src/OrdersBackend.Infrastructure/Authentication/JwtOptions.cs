using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OrdersBackend.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public int ExpirationMinutes { get; init; } = 60;

    public SymmetricSecurityKey CreateSigningKey() => new(Encoding.UTF8.GetBytes(SecretKey));
}
