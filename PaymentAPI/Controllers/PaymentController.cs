using KafkaService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody]ProcessPayment payment)
        {

            return Ok();
        }
    }
}
