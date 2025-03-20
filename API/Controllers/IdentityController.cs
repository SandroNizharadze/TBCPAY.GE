using Application.Commands;
using Application.Handlers;
using Domain.Identity;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class IdentityController : ControllerBase
{
    private readonly TBCPayDbContext _db;

    public IdentityController(TBCPayDbContext db)
    {
        _db = db;
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Identity", null, Request.Scheme);
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl,
            IsPersistent = false,
            AllowRefresh = true,
            ExpiresUtc = DateTime.UtcNow.AddMinutes(5)
        };

        properties.Items["prompt"] = "select_account";
        return Challenge(properties, "google");
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync("cookie");
        if (!result.Succeeded || result.Principal == null)
            return BadRequest(new { Error = result.Failure?.Message ?? "Authentication failed" });

        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
            return BadRequest(new { Error = "No email found" });

        

        var token = GenerateJwtToken(email);
        return Ok(new { Token = token });
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var user = new User(command.Email, BCrypt.Net.BCrypt.HashPassword(command.Password), command.Role, command.PhoneNumber);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok(new { Message = "User registered successfully" });
    }


    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("cookie");
        Response.Cookies.Delete(".AspNetCore.Cookies");
        return Ok(new { Message = "Logged out successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var handler = HttpContext.RequestServices.GetRequiredService<LoginUserCommandHandler>();
        var result = await handler.Handle(command, CancellationToken.None);
        return Ok(new { Message = result, UserId = command.Email });
    }

    private string GenerateJwtToken(string email)
    {
        var key = Encoding.ASCII.GetBytes("kJ9#mP2$vL8@xQ5!wE3&zR6*yT1-uB4!");
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email) }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    [HttpPost("mfa/verify")]
    public async Task<IActionResult> VerifyMfa([FromBody] VerifyMfaCommand command)
    {
        var verificationCode = await _db.VerificationCodes.FirstOrDefaultAsync(vc => vc.UserId == command.UserId);
        if (verificationCode == null || verificationCode.Expiry < DateTime.UtcNow || verificationCode.Code != command.Code)
            return BadRequest(new { Error = "Invalid or expired code" });

        _db.VerificationCodes.Remove(verificationCode);
        await _db.SaveChangesAsync();

        var token = GenerateJwtToken(command.UserId);
        return Ok(new { Token = token });
    }

    public class VerifyMfaCommand
    {
        public required string UserId { get; set; }
        public required string Code { get; set; }
    }


}