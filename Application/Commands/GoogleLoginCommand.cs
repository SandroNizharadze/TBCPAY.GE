using MediatR;

namespace Application.Commands;

public record GoogleLoginCommand(string Email, string Name, string PhoneNumber) : IRequest<string>;
