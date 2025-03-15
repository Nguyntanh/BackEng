using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using register_login.Data;
using register_login.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/auth/login";
            options.LogoutPath = "/auth/logout";
        });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// Add services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register PasswordHasher as scoped service
builder.Services.AddScoped<PasswordHasher<User>>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

// Add controllers
builder.Services.AddControllers();

builder.Services.AddControllersWithViews();


// Configure the application
var app = builder.Build();

// Register endpoints using Minimal APIs
app.MapPost("/api/auth/register", async (RegisterRequest request, AppDbContext db, PasswordHasher<User> hasher) =>
{
    var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
    if (existingUser != null)
        return Results.BadRequest("Username already exists.");

    var user = new User
    {
        Username = request.Username,
        Email = request.Email,
        PasswordHash = hasher.HashPassword(null, request.Password)
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Ok("User registered successfully.");
});

app.MapPost("/api/auth/login", async (LoginRequest request, AppDbContext db, PasswordHasher<User> hasher) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
    if (user == null)
        return Results.NotFound("User not found.");

    var result = hasher.VerifyHashedPassword(null, user.PasswordHash, request.Password);
    if (result == PasswordVerificationResult.Failed)
        return Results.Unauthorized();

    return Results.Ok("Login successful.");
});

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();


// Map controller routes (if you want to use controllers later)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
