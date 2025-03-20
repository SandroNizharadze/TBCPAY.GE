using Application.Handlers;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<TwilioSmsService>();
builder.Services.AddScoped<LoginUserCommandHandler>();
builder.Services.AddScoped<PaymentService>();

builder.Services.AddMediatR(cfg => cfg.AsScoped(), typeof(LoginUserCommandHandler).Assembly);


builder.Services.AddDbContext<TBCPayDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie", o =>
    {
        o.LoginPath = "/login";
        o.Cookie.SameSite = SameSiteMode.None;
        o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    })
    .AddGoogle("google", o =>
    {
        o.ClientId = builder.Configuration["Google:ClientId"] ?? throw new InvalidOperationException("Google client ID is missing");
        o.ClientSecret = builder.Configuration["Google:ClientSecret"] ?? throw new InvalidOperationException("Google client secret is missing");
        o.CallbackPath = "/signin-google"; // Middleware default
        o.SignInScheme = "cookie";
        o.SaveTokens = true;
        o.CorrelationCookie.SameSite = SameSiteMode.None;
        o.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();