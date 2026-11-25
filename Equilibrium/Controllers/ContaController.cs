using Equilibrium.Data;
using Equilibrium.Models;
using Equilibrium.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Equilibrium.Controllers;

public class ContaController : Controller
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<Usuarios> _hasher;

    public ContaController(
        AppDbContext context,
        IPasswordHasher<Usuarios> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    [HttpGet]
    public IActionResult Cadastro()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastro(CadastroViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string email = model.Email.Trim().ToLower();

        if (await _context.Usuarios.AnyAsync(u => u.Email == email))
        {
            ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.CPF) &&
            await _context.Usuarios.AnyAsync(u => u.CPF == model.CPF))
        {
            ModelState.AddModelError("CPF", "Este CPF já está cadastrado.");
            return View(model);
        }

        var paciente = new Paciente(
            model.Nome.Trim(),
            model.CPF,
            email,
            "",
            model.Idade,
            model.Telefone,
            model.Endereco
        );

        paciente.Tipo = "Paciente";
        paciente.Senha = _hasher.HashPassword(paciente, model.Senha);

        _context.Pacientes.Add(paciente);

        await _context.SaveChangesAsync();

        TempData["Mensagem"] = "Cadastro realizado! Faça seu login.";

        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string email = model.Email.Trim().ToLower();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);

        if (usuario == null)
        {
            ModelState.AddModelError("", "E-mail ou senha incorretos.");
            return View(model);
        }

        var resultado = _hasher.VerifyHashedPassword(
            usuario,
            usuario.Senha,
            model.Senha
        );

        if (resultado == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "E-mail ou senha incorretos.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Tipo)
        };

        var identidade = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identidade);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        return usuario.Tipo switch
        {
            "Paciente" => RedirectToAction("Index", "Paciente"),
            "Medico" => RedirectToAction("Index", "Medico"),
            "Admin" => RedirectToAction("Index", "Admin"),
            _ => RedirectToAction("Login")
        };
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return RedirectToAction("Login");
    }
}