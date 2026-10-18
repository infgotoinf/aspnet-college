using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoManager.Data;
using TodoManager.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services
    .AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var signingKey = jwtSettings["SigningKey"]
    ?? throw new InvalidOperationException("JWT SigningKey is not configured.");

builder.Services
    .AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.ConfigureFilter(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddControllers();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        var headers = context.Response.Headers;

        headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'";
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["Referrer-Policy"] = "no-referrer";

        if (context.Request.IsHttps)
        {
            headers["Strict-Transport-Security"] =
                "max-age=31536000; includeSubDomains";
        }

        return Task.CompletedTask;
    });

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();

    await db.Database.EnsureCreatedAsync();

    await db.Database.ExecuteSqlRawAsync("""
        CREATE TABLE IF NOT EXISTS "RefreshTokens" (
            "Id" INTEGER NOT NULL CONSTRAINT "PK_RefreshTokens" PRIMARY KEY AUTOINCREMENT,
            "Token" TEXT NOT NULL,
            "UserId" TEXT NOT NULL,
            "ExpiresAt" TEXT NOT NULL,
            "RevokedAt" TEXT NULL,
            "CreatedAt" TEXT NOT NULL,
            CONSTRAINT "FK_RefreshTokens_AspNetUsers_UserId"
                FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
        );

        CREATE UNIQUE INDEX IF NOT EXISTS "IX_RefreshTokens_Token"
            ON "RefreshTokens" ("Token");
        """);

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var role in new[] { "Admin", "Author", "User" })
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(role));

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Could not create role '{role}': " +
                    string.Join("; ", result.Errors.Select(error => error.Description)));
            }
        }
    }

    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    const string adminEmail = "admin@todo.com";

    var admin = await userManager.FindByEmailAsync(adminEmail);

    if (admin is null)
    {
        admin = new AppUser
        {
            FullName = "Admin",
            UserName = adminEmail,
            Email = adminEmail
        };

        var result = await userManager.CreateAsync(admin, "Admin123!");

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "Could not create the seed admin: " +
                string.Join("; ", result.Errors.Select(error => error.Description)));
        }
    }

    if (!await userManager.IsInRoleAsync(admin, "Admin"))
    {
        var result = await userManager.AddToRoleAsync(admin, "Admin");

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "Could not assign the Admin role: " +
                string.Join("; ", result.Errors.Select(error => error.Description)));
        }
    }
}

app.Run();
