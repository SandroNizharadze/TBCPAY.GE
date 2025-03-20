using Stripe;
using Stripe.Checkout;

public class PaymentService
{
    private readonly IConfiguration _config;

    public PaymentService(IConfiguration config)
    {
        _config = config;
        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
    }

    public async Task<string> CreateStripePayment(decimal amount, string currency)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = currency.ToLower(),
                        ProductData = new SessionLineItemPriceDataProductDataOptions { Name = "TBCPAY Transaction" },
                        UnitAmount = (long)(amount * 100) // Convert to cents
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = "http://localhost:5000/success?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = "http://localhost:5000/cancel" 
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(options);
        return session.Url;
    }
}