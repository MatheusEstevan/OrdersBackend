using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using OrdersBackend.Application.Common.Exceptions;
using OrdersBackend.Domain.Common;

namespace OrdersBackend.Api.Exceptions;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        IResult? result = exception switch
        {
            ValidationException ex => Results.ValidationProblem(
                ex.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),

            DomainException ex => Results.Problem(ex.Message, statusCode: StatusCodes.Status422UnprocessableEntity),

            NotFoundException ex => Results.Problem(ex.Message, statusCode: StatusCodes.Status404NotFound),

            InvalidCredentialsException ex => Results.Problem(ex.Message, statusCode: StatusCodes.Status401Unauthorized),

            BadHttpRequestException ex => Results.Problem("Requisição inválida. Verifique o corpo e os parâmetros enviados.", statusCode: ex.StatusCode),

            _ => null
        };

        if (result is null)
            return false;

        await result.ExecuteAsync(httpContext);
        return true;
    }
}