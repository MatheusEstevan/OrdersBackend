using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using OrdersBackend.Application.Auth.Commands.Login;

namespace OrdersBackend.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Auth")
            .AllowAnonymous();

        group.MapPost("/login", Login);

        return app;
    }

    private static async Task<Ok<LoginResponse>> Login(
        LoginCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(response);
    }
}
