using System;

namespace Domain.Identity.Events;

public record class UserRegistered(int UserId, string Email);
