using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Equilibrium.Data;
using Equilibrium.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Equilibrium.Controllers;

public class ContaController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<Usuarios> _hasher;
    private readonly IDataProtector _resetProtector;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public ContaController(AppDbContext db, IPasswordHasher<Usuarios> hasher,
        IDataProtectionProvider protection, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _db = db;
        _hasher = hasher;
        _resetProtector = protection.CreateProtector("Equilibrium.PasswordReset.v1");
        _configuration = configuration;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { Email = string.Empty });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);
        var usuario = await _db.Usuarios.SingleOrDefaultAsync(u => u.Email == model.Email.Trim());
        if (usuario is null || _hasher.VerifyHashedPassword(usuario, usuario.Senha, model.Senha) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.Tipo)
        };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl!);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Cadastro() => View(new CadastroViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastro(CadastroViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var email = model.Email.Trim();
        var cpf = new string(model.CPF.Where(char.IsDigit).ToArray());
        var emailEmUso = await _db.Usuarios.AnyAsync(u => u.Email == email);
        var cpfEmUso = await _db.Usuarios.AnyAsync(u => u.CPF == cpf);
        if (emailEmUso || cpfEmUso)
            ModelState.AddModelError(string.Empty, "Não foi possível concluir o cadastro. Verifique os dados informados.");
        if (!ModelState.IsValid) return View(model);

        var paciente = new Paciente(model.Nome.Trim(), cpf, email, string.Empty,
            model.Idade, model.Telefone?.Trim() ?? string.Empty, model.Endereco?.Trim() ?? string.Empty);
        paciente.Senha = _hasher.HashPassword(paciente, model.Senha);
        _db.Pacientes.Add(paciente);
        await _db.SaveChangesAsync();
        TempData["Mensagem"] = "Cadastro realizado. Entre com seu e-mail e senha.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult EsqueciSenha() => View(new SolicitarRedefinicaoViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EsqueciSenha(SolicitarRedefinicaoViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        // A recuperação pública nunca pode alterar a conta reservada à dona do site.
        var usuario = await _db.Usuarios.SingleOrDefaultAsync(u => u.Email == model.Email.Trim() && u.Tipo != "Admin");
        string? link = null;
        if (usuario is not null)
        {
            var payload = $"{usuario.Id}|{DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds()}";
            var token = WebUtility.UrlEncode(_resetProtector.Protect(payload));
            link = Url.Action(nameof(RedefinirSenha), "Conta", new { email = usuario.Email, token }, Request.Scheme);
            await EnviarLinkAsync(usuario.Email, link!);
        }

        TempData["Mensagem"] = "Se o e-mail estiver cadastrado, enviaremos as instruções para redefinir sua senha.";
        if (_environment.IsDevelopment() && link is not null && !EmailConfigurado())
            TempData["LinkDesenvolvimento"] = link;
        return RedirectToAction(nameof(EsqueciSenha));
    }

    [HttpGet]
    public IActionResult RedefinirSenha(string email, string token) =>
        View(new RedefinirSenhaViewModel { Email = email, Token = token });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RedefinirSenha(RedefinirSenhaViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var usuario = await _db.Usuarios.SingleOrDefaultAsync(u => u.Email == model.Email && u.Tipo != "Admin");
        if (usuario is null || !TokenValido(model.Token, usuario.Id))
        {
            ModelState.AddModelError(string.Empty, "O link expirou ou é inválido. Solicite uma nova redefinição.");
            return View(model);
        }
        usuario.Senha = _hasher.HashPassword(usuario, model.NovaSenha);
        await _db.SaveChangesAsync();
        TempData["Mensagem"] = "Senha atualizada. Você já pode entrar na sua conta.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Sair()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    private bool TokenValido(string token, int usuarioId)
    {
        try
        {
            var dados = _resetProtector.Unprotect(WebUtility.UrlDecode(token)).Split('|');
            return dados.Length == 2 && int.Parse(dados[0]) == usuarioId &&
                DateTimeOffset.UtcNow.ToUnixTimeSeconds() <= long.Parse(dados[1]);
        }
        catch { return false; }
    }

    private bool EmailConfigurado() => _configuration.GetValue<int?>("Email:Smtp:Port") is not null &&
        !string.IsNullOrWhiteSpace(_configuration["Email:Smtp:Host"]);

    private async Task EnviarLinkAsync(string destino, string link)
    {
        if (!EmailConfigurado()) return;
        var smtp = _configuration.GetSection("Email:Smtp");
        using var mensagem = new MailMessage(smtp["From"]!, destino,
            "Redefinição de senha - Equilibrium", $"Acesse este link em até 30 minutos para criar uma nova senha: {link}");
        using var cliente = new SmtpClient(smtp["Host"]!, smtp.GetValue<int>("Port"))
        {
            EnableSsl = smtp.GetValue<bool>("EnableSsl"),
            Credentials = new NetworkCredential(smtp["User"], smtp["Password"])
        };
        await cliente.SendMailAsync(mensagem);
    }
}
