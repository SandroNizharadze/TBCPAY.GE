using Application.Commands;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> ProcessStripePayment([FromBody] ProcessPaymentCommand request)
    {
        var sessionUrl = await _paymentService.CreateStripePayment(request.Amount, request.Currency);
        return Ok(new { Url = sessionUrl });
    }
}