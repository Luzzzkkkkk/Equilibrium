using Equilibrium.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Equilibrium.Controllers;

[Authorize(Roles = "Medico")]
public class MedicoController : Controller
{
    private readonly AppDbContext _context;

    public MedicoController(AppDbContext context)
    {
        _context = context;
    }

    private int UsuarioId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var medico = await _context.Medicos
            .FirstOrDefaultAsync(m => m.Id == UsuarioId);

        if (medico == null)
            return NotFound();

        return View(medico);
    }

    public async Task<IActionResult> Consultas()
    {
        var consultas = await _context.Consultas
            .Include(c => c.Paciente)
            .Where(c => c.MedicoId == UsuarioId)
            .OrderBy(c => c.DataHora)
            .ToListAsync();

        return View(consultas);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarStatus(
        int id,
        string status)
    {
        var statusPermitidos = new[]
        {
            "Confirmada",
            "Realizada",
            "Cancelada"
        };

        if (!statusPermitidos.Contains(status))
            return BadRequest();

        var consulta = await _context.Consultas
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.MedicoId == UsuarioId
            );

        if (consulta == null)
            return NotFound();

        consulta.Status = status;

        await _context.SaveChangesAsync();

        return RedirectToAction("Consultas");
    }
}