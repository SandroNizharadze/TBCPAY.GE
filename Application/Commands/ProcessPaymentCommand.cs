using System;
using MediatR;

namespace Application.Commands;

public class ProcessPaymentCommand : IRequest<Guid>
{
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public required string PaymentMethod { get; set; }
}