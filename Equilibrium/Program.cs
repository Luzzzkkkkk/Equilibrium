using Equilibrium.Data;
using Equilibrium.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(
        builder.Configuration.GetConnectionString("ConexaoMySQL")
        ?? throw new InvalidOperationException("Configure a conexão ConexaoMySQL.")));
builder.Services.AddScoped<IPasswordHasher<Usuarios>, PasswordHasher<Usuarios>>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Conta/Login";
        options.AccessDeniedPath = "/Conta/Login";
        options.Cookie.Name = "Equilibrium.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    var adminEmail = app.Configuration["AdminSeed:Email"];
    var adminPassword = app.Configuration["AdminSeed:Password"];
    if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
    {
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuarios>>();
        var admin = await db.Usuarios.SingleOrDefaultAsync(u => u.Email == adminEmail);
        if (admin is null)
        {
            admin = new Administrador("Administrador", null, adminEmail, string.Empty, "Total");
            admin.Senha = hasher.HashPassword(admin, adminPassword);
            db.Administradores.Add((Administrador)admin);
        }
        else
        {
            admin.Tipo = "Admin";
            if (hasher.VerifyHashedPassword(admin, admin.Senha, adminPassword) == PasswordVerificationResult.Failed)
                admin.Senha = hasher.HashPassword(admin, adminPassword);
        }

        await db.SaveChangesAsync();
    }
}

app.Run();
