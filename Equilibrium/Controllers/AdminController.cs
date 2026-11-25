using Equilibrium.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Equilibrium.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalUsuarios = await _context.Usuarios.CountAsync();
        ViewBag.TotalPacientes = await _context.Pacientes.CountAsync();
        ViewBag.TotalMedicos = await _context.Medicos.CountAsync();
        ViewBag.TotalConsultas = await _context.Consultas.CountAsync();

        return View();
    }

    public async Task<IActionResult> Usuarios()
    {
        var usuarios = await _context.Usuarios
            .OrderBy(u => u.Nome)
            .ToListAsync();

        return View(usuarios);
    }

    public async Task<IActionResult> Consultas()
    {
        var consultas = await _context.Consultas
            .Include(c => c.Paciente)
            .Include(c => c.Medico)
            .OrderBy(c => c.DataHora)
            .ToListAsync();

        return View(consultas);
    }
}