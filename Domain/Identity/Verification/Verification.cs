using System;

namespace Domain.Identity.Verification;

public class VerificationCode
{
    public required string UserId { get; set; }
    public required string Code { get; set; }
    public DateTime Expiry { get; set; }
}
