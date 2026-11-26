using System.Security.Claims;
using Equilibrium.Data;
using Equilibrium.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Equilibrium.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private int UsuarioId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    public HomeController(AppDbContext context) => _context = context;

    public IActionResult Index() => User.Identity?.IsAuthenticated == true
        ? RedirectToAction(nameof(MeuEspaco))
        : RedirectToAction("Login", "Conta");

    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> MeuEspaco()
    {
        var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.Id == UsuarioId);
        return paciente is null ? NotFound() : View(paciente);
    }

    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> Profissionais(string? especialidade)
    {
        var especialidades = await _context.Medicos.Select(m => m.Especialidade)
            .Distinct().OrderBy(e => e).ToListAsync();
        ViewBag.Especialidades = especialidades;
        ViewBag.Especialidade = especialidade;
        var medicos = _context.Medicos.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(especialidade))
            medicos = medicos.Where(m => m.Especialidade == especialidade);
        return View(await medicos.OrderBy(m => m.Nome).ToListAsync());
    }

    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> Consultas()
    {
        var consultas = await _context.Consultas.Include(c => c.Medico)
            .Where(c => c.PacienteId == UsuarioId).OrderBy(c => c.DataHora).ToListAsync();
        return View(consultas);
    }

    [Authorize(Roles = "Paciente")]
    public IActionResult BemEstar() => View();

    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> Configuracoes()
    {
        var paciente = await _context.Pacientes.FindAsync(UsuarioId);
        return paciente is null ? NotFound() : View(paciente);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Paciente")]
    public async Task<IActionResult> AtualizarPerfil(string nome, int idade, string? telefone, string? endereco)
    {
        var paciente = await _context.Pacientes.FindAsync(UsuarioId);
        if (paciente is null) return NotFound();
        var telefoneNumeros = DadosPessoais.SomenteNumeros(telefone);
        if (string.IsNullOrWhiteSpace(nome) || nome.Length > 100 || idade is < 1 or > 120 ||
            (telefoneNumeros.Length != 0 && telefoneNumeros.Length is not (10 or 11)) ||
            (endereco?.Length ?? 0) > 200)
        {
            TempData["Erro"] = "Confira os dados informados e tente novamente.";
            return RedirectToAction(nameof(Configuracoes));
        }
        paciente.Nome = nome.Trim();
        paciente.Idade = idade;
        paciente.Telefone = telefoneNumeros;
        paciente.Endereco = endereco?.Trim() ?? string.Empty;
        await _context.SaveChangesAsync();
        TempData["Mensagem"] = "Dados atualizados com sucesso.";
        return RedirectToAction(nameof(Configuracoes));
    }

    public IActionResult Privacy() => View();
    public IActionResult Error() => View();
}
