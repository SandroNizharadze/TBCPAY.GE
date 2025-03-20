using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;
using System;
using Twilio;

namespace API.Services;

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
