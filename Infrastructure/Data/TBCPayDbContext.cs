using System;
using Microsoft.EntityFrameworkCore;
using Domain.Identity;
using Domain.Identity.Verification;
using Domain.Identity.Payments;
namespace Infrastructure.Data;

public class TBCPayDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<Payment> Payments { get; set; } 
    public TBCPayDbContext(DbContextOptions<TBCPayDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<VerificationCode>().HasKey(vc => vc.UserId);
        modelBuilder.Entity<Payment>().HasKey(t => t.Id);
    }
}
