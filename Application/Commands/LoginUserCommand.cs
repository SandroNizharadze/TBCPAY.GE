using System;

namespace Application.Commands;

public record LoginUserCommand(string Email, string Password);
