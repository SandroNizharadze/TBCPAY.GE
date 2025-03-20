using System;

namespace Domain.Identity;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public string PhoneNumber { get; set; }
    
    public User(string email, string passwordHash, string role, string phoneNumber)
    {
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        PhoneNumber = phoneNumber;
    }
}