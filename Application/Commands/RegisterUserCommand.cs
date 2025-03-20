using System;

namespace Application.Commands;

public class RegisterUserCommand
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
    public required string PhoneNumber { get; set; }
}