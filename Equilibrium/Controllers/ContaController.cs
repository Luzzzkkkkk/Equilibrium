using Equilibrium.Data;
using Equilibrium.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Equilibrium.Controllers;

public class ContaController : Controller
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<Usuarios> _hasher;
    private readonly IDataProtector _resetProtector;

    public ContaController(
        AppDbContext context,
        IPasswordHasher<Usuarios> hasher,
        IDataProtectionProvider protectionProvider)
    {
        _context = context;
        _hasher = hasher;
        _resetProtector = protectionProvider.CreateProtector("Equilibrium.PasswordReset.v1");
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
        string cpf = DadosPessoais.SomenteNumeros(model.CPF);
        string telefone = DadosPessoais.SomenteNumeros(model.Telefone);

        if (await _context.Usuarios.AnyAsync(u => u.Email == email))
        {
            ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
            return View(model);
        }

        if (await _context.Usuarios.AnyAsync(u => u.CPF == cpf))
        {
            ModelState.AddModelError("CPF", "Este CPF já está cadastrado.");
            return View(model);
        }

        var paciente = new Paciente(
            model.Nome.Trim(),
            cpf,
            email,
            "",
            model.Idade,
            telefone,
            model.Endereco ?? string.Empty
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
            "Paciente" => RedirectToAction("MeuEspaco", "Home"),
            "Medico" => RedirectToAction("Index", "Medico"),
            "Admin" => RedirectToAction("Index", "Admin"),
            _ => RedirectToAction("Login")
        };
    }

    [HttpGet]
    public IActionResult EsqueciSenha() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EsqueciSenha(SolicitarRedefinicaoViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var email = model.Email.Trim().ToLowerInvariant();
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        TempData["Mensagem"] = "Se o e-mail estiver cadastrado, as instruções estarão disponíveis aqui.";
        if (usuario is not null)
        {
            var payload = $"{usuario.Email}|{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(_resetProtector.Protect(payload)));
            TempData["LinkDesenvolvimento"] = Url.Action(nameof(RedefinirSenha), "Conta", new { token, email = usuario.Email }, Request.Scheme);
        }
        return View();
    }

    [HttpGet]
    public IActionResult RedefinirSenha(string email, string token) => View(new RedefinirSenhaViewModel { Email = email, Token = token });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RedefinirSenha(RedefinirSenhaViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        try
        {
            var protectedValue = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var payload = _resetProtector.Unprotect(protectedValue).Split('|');
            if (payload.Length != 2 || !string.Equals(payload[0], model.Email, StringComparison.OrdinalIgnoreCase) ||
                DateTimeOffset.UtcNow - DateTimeOffset.FromUnixTimeSeconds(long.Parse(payload[1])) > TimeSpan.FromMinutes(30))
                throw new InvalidOperationException();
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Este link expirou ou não é válido. Solicite outro.");
            return View(model);
        }

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (usuario is null) return NotFound();
        usuario.Senha = _hasher.HashPassword(usuario, model.NovaSenha);
        await _context.SaveChangesAsync();
        TempData["Mensagem"] = "Senha alterada. Entre com sua nova senha.";
        return RedirectToAction(nameof(Login));
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
