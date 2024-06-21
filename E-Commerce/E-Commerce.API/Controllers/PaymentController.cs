using E_Commerce.API.Dtos;
using E_Commerce.API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto request)
        {
            _logger.LogInformation("Processing payment for amount: {Amount}", request.Amount);
            try
            {
                var authorizationUrl = await _paymentService.ProcessPayment(request);
                _logger.LogInformation("Payment initialization successful for amount: {Amount}", request.Amount);
                return Ok(new { AuthorizationUrl = authorizationUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment initialization failed for amount: {Amount}", request.Amount);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("verify/{txRef}")]
        public async Task<IActionResult> VerifyPayment(string txRef)
        {
            _logger.LogInformation("Verifying payment with reference: {Reference}", txRef);
            try
            {
                var result = await _paymentService.VerifyPayment(txRef);
                _logger.LogInformation("Payment verification result: {Result}", result);
                return Ok(new { Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment verification failed for reference: {Reference}", txRef);
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
