using System;

namespace Domain.Identity.Payments;

public class Payment
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public required string Status { get; set; } // Pending, Completed, Failed
    public required string PaymentMethod { get; set; } // Visa, MasterCard, ApplePay, etc.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}