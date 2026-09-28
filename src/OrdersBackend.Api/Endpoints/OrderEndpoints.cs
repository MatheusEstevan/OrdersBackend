using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using OrdersBackend.Application.Orders.Commands.CancelOrder;
using OrdersBackend.Application.Orders.Commands.CreateOrder;
using OrdersBackend.Application.Orders.Dtos;
using OrdersBackend.Application.Orders.Queries.GetAllOrders;
using OrdersBackend.Application.Orders.Queries.GetOrderById;

namespace OrdersBackend.Api.Endpoints
{
    public static class OrderEndpoints
    {
        public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/orders")
                .WithTags("Orders")
                .RequireAuthorization();
            group.MapPost("/", CreateOrder);
            group.MapGet("/", GetAllOrders);
            group.MapGet("/{id:guid}", GetOrderById);
            group.MapPatch("/{id:guid}/cancel", CancelOrder);
            return app;
        }

        private static async Task<Created<CreateOrderResponse>> CreateOrder(
        CreateOrderCommand command,
        ISender sender,
        CancellationToken cancellationToken)
        {
            var orderId = await sender.Send(command, cancellationToken);
            return TypedResults.Created($"/api/orders/{orderId}", new CreateOrderResponse(orderId));
        }

        private static async Task<Ok<GetAllOrdersResponse>> GetAllOrders(
            ISender sender,
            CancellationToken cancellationToken,
            int page = 1,
            int pageSize = 10)
        {
            var orders = await sender.Send(new GetAllOrdersQuery(page, pageSize), cancellationToken);
            return TypedResults.Ok(orders);
        }

        private static async Task<Ok<OrderDto>> GetOrderById(
            Guid id,
            ISender sender,
            CancellationToken cancellationToken)
        {
            var order = await sender.Send(new GetOrderByIdQuery(id), cancellationToken);
            return TypedResults.Ok(order);
        }

        private static async Task<NoContent> CancelOrder(
            Guid id,
            ISender sender,
            CancellationToken cancellationToken)
        {
            await sender.Send(new CancelOrderCommand(id), cancellationToken);
            return TypedResults.NoContent();
        }
    }

    public sealed record CreateOrderResponse(Guid Id);

}
