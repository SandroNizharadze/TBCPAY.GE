using System;
using Application.Commands;
using Domain.Identity.Payments;
using Infrastructure.Data;
using MediatR;

namespace Application.Handlers;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Guid>
{
    private readonly TBCPayDbContext _dbContext;

    public ProcessPaymentCommandHandler(TBCPayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Amount = request.Amount,
            Currency = request.Currency,
            PaymentMethod = request.PaymentMethod,
            Status = "Pending"
        };

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
}