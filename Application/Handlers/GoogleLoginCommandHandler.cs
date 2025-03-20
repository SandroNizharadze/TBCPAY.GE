using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Commands;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Handlers;

public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, string>
{
    private readonly TBCPayDbContext _db;
    private readonly IConfiguration _configuration;

    public GoogleLoginCommandHandler(TBCPayDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<string> Handle(GoogleLoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);
        if (user == null)
        {
            user = new Domain.Identity.User(command.Email, BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), "Customer", command.PhoneNumber);
            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT secret is missing"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Role, user.Role) }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}