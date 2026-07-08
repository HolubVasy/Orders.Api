using Microsoft.AspNetCore.Mvc;

namespace Orders.Api;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CancellationToken cancellationToken)
    {
        var orderId = await _orderService.CreateAsync(cancellationToken);

        return Ok(new
        {
            OrderId = orderId,
            Status = "Created"
        });
    }
}