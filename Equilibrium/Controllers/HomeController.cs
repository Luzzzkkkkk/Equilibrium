using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Equilibrium.Models;
using Equilibrium.Data;
using Microsoft.EntityFrameworkCore;

namespace Equilibrium.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db) => _db = db;

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Profissionais(string? especialidade)
    {
        var query = _db.Medicos.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(especialidade))
            query = query.Where(m => m.Especialidade == especialidade);

        ViewBag.Especialidade = especialidade;
        return View(await query.OrderBy(m => m.Nome).ToListAsync());
    }

    public IActionResult MeuEspaco() => View();

    public IActionResult Consultas() => View();

    public IActionResult BemEstar() => View();

    public IActionResult Configuracoes() => View();

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
