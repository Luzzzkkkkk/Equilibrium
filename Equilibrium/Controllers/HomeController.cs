using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Equilibrium.Models;

namespace Equilibrium.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Profissionais() => View();

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
