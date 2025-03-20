using Application.Commands;
using Domain.Identity.Verification;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace Application.Handlers
{
    public class LoginUserCommandHandler
    {
        private readonly TBCPayDbContext _db;
        private readonly TwilioSmsService _smsService;
        private readonly string? _jwtSecret;

        public LoginUserCommandHandler(TBCPayDbContext db, TwilioSmsService smsService, IConfiguration configuration)
        {
            _db = db;
            _smsService = smsService;
            _jwtSecret = configuration["Jwt:Secret"];
        }

        public async Task<string> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);
            if (user == null || !BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
                throw new Exception("Invalid email or password");

            if (string.IsNullOrEmpty(user.PhoneNumber))
                throw new Exception("Phone number not set for user");

            // Generate and send MFA code
            var code = new Random().Next(100000, 999999).ToString();
            var verificationCode = new VerificationCode
            {
                UserId = user.Email,
                Code = code,
                Expiry = DateTime.UtcNow.AddMinutes(5)
            };

            var existingCode = await _db.VerificationCodes.FindAsync(user.Email);
            if (existingCode != null)
                _db.VerificationCodes.Remove(existingCode);
            _db.VerificationCodes.Add(verificationCode);
            await _db.SaveChangesAsync(cancellationToken);

            await _smsService.SendSmsAsync(user.PhoneNumber, code);
            return "MFA code sent";
        }
    }

    public class TwilioSmsService
    {
        private readonly IConfiguration _config;

        public TwilioSmsService(IConfiguration config)
        {
            _config = config;
            TwilioClient.Init(_config["Twilio:AccountSid"], _config["Twilio:AuthToken"]);
        }

        public async Task SendSmsAsync(string toPhoneNumber, string code)
        {
            var message = await MessageResource.CreateAsync(
                to: new PhoneNumber(toPhoneNumber),
                from: new PhoneNumber(_config["Twilio:PhoneNumber"]),
                body: $"Your verification code is: {code}"
            );
        }
    }
}