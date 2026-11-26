using Equilibrium.Data;
using Equilibrium.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Equilibrium.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<Usuarios> _hasher;

    public AdminController(AppDbContext context, IPasswordHasher<Usuarios> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalUsuarios = await _context.Usuarios.CountAsync();
        ViewBag.TotalPacientes = await _context.Pacientes.CountAsync();
        ViewBag.TotalMedicos = await _context.Medicos.CountAsync();
        ViewBag.TotalConsultas = await _context.Consultas.CountAsync();
        return View();
    }

    public async Task<IActionResult> Profissionais()
    {
        var profissionais = await _context.Medicos.OrderBy(m => m.Nome).ToListAsync();
        return View(profissionais);
    }

    [HttpGet]
    public IActionResult NovoProfissional() => View(new CadastroProfissionalViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> NovoProfissional(CadastroProfissionalViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var email = model.Email.Trim().ToLowerInvariant();
        var cpf = DadosPessoais.SomenteNumeros(model.CPF);
        var crm = model.CRM.Trim();

        if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            ModelState.AddModelError(nameof(model.Email), "Este e-mail já está cadastrado.");
        if (await _context.Usuarios.AnyAsync(u => u.CPF == cpf))
            ModelState.AddModelError(nameof(model.CPF), "Este CPF já está cadastrado.");
        if (await _context.Medicos.AnyAsync(m => m.CRM == crm))
            ModelState.AddModelError(nameof(model.CRM), "Este CRM já está cadastrado.");
        if (!ModelState.IsValid) return View(model);

        var medico = new Medico(model.Nome.Trim(), cpf, email, string.Empty, crm,
            model.Especialidade.Trim());
        medico.Senha = _hasher.HashPassword(medico, model.Senha);
        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(CadastroConcluido));
    }

    [HttpGet]
    public IActionResult CadastroConcluido() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirProfissional(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);
        if (medico is null) return NotFound();

        if (await _context.Consultas.AnyAsync(c => c.MedicoId == id))
        {
            TempData["Erro"] = "Este profissional tem consultas vinculadas. Cancele ou remova essas consultas antes de excluí-lo.";
            return RedirectToAction(nameof(Profissionais));
        }

        _context.Medicos.Remove(medico);
        await _context.SaveChangesAsync();
        TempData["Mensagem"] = "Profissional excluído.";
        return RedirectToAction(nameof(Profissionais));
    }

    public async Task<IActionResult> Usuarios()
    {
        var usuarios = await _context.Usuarios.OrderBy(u => u.Nome).ToListAsync();
        return View(usuarios);
    }

    public async Task<IActionResult> Consultas()
    {
        var consultas = await _context.Consultas.Include(c => c.Paciente)
            .Include(c => c.Medico).OrderBy(c => c.DataHora).ToListAsync();
        return View(consultas);
    }
}
