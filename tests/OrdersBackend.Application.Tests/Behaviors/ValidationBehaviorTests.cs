using FluentValidation;
using MediatR;
using OrdersBackend.Application.Behaviors;

namespace OrdersBackend.Application.Tests.Behaviors;

public class ValidationBehaviorTests
{
    public sealed record TestRequest(string Name, int Amount) : IRequest<string>;

    private const string HandlerResponse = "handled";

    private int _nextCalls;

    private Task<string> Next(CancellationToken cancellationToken)
    {
        _nextCalls++;
        return Task.FromResult(HandlerResponse);
    }

    private static InlineValidator<TestRequest> NameRequired() =>
        new() { v => v.RuleFor(r => r.Name).NotEmpty() };

    private static InlineValidator<TestRequest> AmountPositive() =>
        new() { v => v.RuleFor(r => r.Amount).GreaterThan(0) };

    private static ValidationBehavior<TestRequest, string> CreateBehavior(params IValidator<TestRequest>[] validators) =>
        new(validators);

    [Fact]
    public async Task Handle_WithoutValidators_CallsNextAndReturnsItsResponse()
    {
        var behavior = CreateBehavior();

        var response = await behavior.Handle(new TestRequest("", 0), Next, CancellationToken.None);

        Assert.Equal(HandlerResponse, response);
        Assert.Equal(1, _nextCalls);
    }

    [Fact]
    public async Task Handle_WithValidRequest_CallsNextOnce()
    {
        var behavior = CreateBehavior(NameRequired(), AmountPositive());

        var response = await behavior.Handle(new TestRequest("Order", 1), Next, CancellationToken.None);

        Assert.Equal(HandlerResponse, response);
        Assert.Equal(1, _nextCalls);
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ThrowsValidationExceptionAndDoesNotCallNext()
    {
        var behavior = CreateBehavior(NameRequired());

        await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(new TestRequest("", 1), Next, CancellationToken.None));

        Assert.Equal(0, _nextCalls);
    }

    [Fact]
    public async Task Handle_WithSeveralFailingValidators_ReportsErrorsFromAllOfThem()
    {
        var behavior = CreateBehavior(NameRequired(), AmountPositive());

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(new TestRequest("", 0), Next, CancellationToken.None));

        Assert.Contains(exception.Errors, e => e.PropertyName == nameof(TestRequest.Name));
        Assert.Contains(exception.Errors, e => e.PropertyName == nameof(TestRequest.Amount));
        Assert.Equal(0, _nextCalls);
    }
}
