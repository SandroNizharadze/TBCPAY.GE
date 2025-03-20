using System;
using Application.Commands;
using Domain.Identity;
using Infrastructure.Data;

namespace Application.Handlers;

public class RegisterUserCommandHandler
{
    public readonly TBCPayDbContext _db;

    public RegisterUserCommandHandler(TBCPayDbContext db) => _db = db;

    public async Task Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);
        var user = new User(command.Email, passwordHash, command.Role, command.PhoneNumber);
        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        // Raise UserRegistered event for later
    }
}
