using Equilibrium.Data;
using Equilibrium.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Equilibrium.Controllers;

[Authorize(Roles = "Paciente")]
public class PacienteController : Controller
{
    private readonly AppDbContext _context;

    public PacienteController(AppDbContext context)
    {
        _context = context;
    }

    private int UsuarioId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var paciente = await _context.Pacientes
            .Include(p => p.Prontuarios)
            .FirstOrDefaultAsync(p => p.Id == UsuarioId);

        if (paciente == null)
            return NotFound();

        return View(paciente);
    }

    public async Task<IActionResult> Consultas()
    {
        var consultas = await _context.Consultas
            .Include(c => c.Medico)
            .Where(c => c.PacienteId == UsuarioId)
            .OrderBy(c => c.DataHora)
            .ToListAsync();

        return View(consultas);
    }

    [HttpGet]
    public async Task<IActionResult> Agendar()
    {
        await CarregarMedicos();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Agendar(
        int medicoId,
        DateTime dataHora)
    {
        if (dataHora <= DateTime.Now)
        {
            ModelState.AddModelError(
                "",
                "Escolha uma data e horário futuros."
            );

            await CarregarMedicos();
            return View();
        }

        var medico = await _context.Medicos
            .FirstOrDefaultAsync(m => m.Id == medicoId);

        if (medico == null)
        {
            ModelState.AddModelError("", "Médico não encontrado.");
            await CarregarMedicos();
            return View();
        }

        bool horarioOcupado = await _context.Consultas.AnyAsync(c =>
            c.MedicoId == medicoId &&
            c.DataHora == dataHora &&
            c.Status != "Cancelada"
        );

        if (horarioOcupado)
        {
            ModelState.AddModelError(
                "",
                "Este horário não está disponível."
            );

            await CarregarMedicos();
            return View();
        }

        var consulta = new Consulta
        {
            PacienteId = UsuarioId,
            MedicoId = medicoId,
            DataHora = dataHora,
            Status = "Pendente"
        };

        _context.Consultas.Add(consulta);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                "",
                "Não foi possível reservar esse horário. Tente outro."
            );

            await CarregarMedicos();
            return View();
        }

        TempData["Mensagem"] = "Solicitação de consulta enviada!";

        return RedirectToAction("Consultas");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var consulta = await _context.Consultas
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.PacienteId == UsuarioId
            );

        if (consulta == null)
            return NotFound();

        if (consulta.Status == "Realizada")
            return BadRequest("Não é possível cancelar uma consulta realizada.");

        consulta.Status = "Cancelada";

        await _context.SaveChangesAsync();

        return RedirectToAction("Consultas");
    }

    private async Task CarregarMedicos()
    {
        var medicos = await _context.Medicos
            .OrderBy(m => m.Nome)
            .Select(m => new
            {
                m.Id,
                Nome = m.Nome + " - " + m.Especialidade
            })
            .ToListAsync();

        ViewBag.Medicos = new SelectList(medicos, "Id", "Nome");
    }
}