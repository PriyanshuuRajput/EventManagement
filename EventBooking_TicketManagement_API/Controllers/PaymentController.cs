using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;

namespace EventBooking_TicketManagement_API.Controllers
{

    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration  _configuration;

        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }

        [HttpPost("create-order")]
        public IActionResult CreateOrder([FromBody] decimal amount)
        {
            try
            {
                var key = _configuration["Razorpay:KeyId"];
                var secret = _configuration["Razorpay:KeySecret"];

                var client = new RazorpayClient(key, secret);

                var options = new Dictionary<string, object>
        {
            { "amount", (int)(amount * 100) },
            { "currency", "INR" },
            { "receipt", Guid.NewGuid().ToString() }
        };

                var order = client.Order.Create(options);

                return Ok(new
                {
                    orderId = order["id"].ToString(),
                    key = key
                });
            }
            catch (Razorpay.Api.Errors.BadRequestError ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
