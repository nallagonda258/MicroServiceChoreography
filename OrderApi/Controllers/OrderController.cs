using KafkaService.Models;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Impl;
using System.Threading.Tasks;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost("createorder")]
        public async Task<IActionResult> CreateOrder([FromBody]CreateOrderRequest request)
        {
            var result = await _orderService.CreateOrderAsync(request);

            return Ok(result);
        }

    }
}
