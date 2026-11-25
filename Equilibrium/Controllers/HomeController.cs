using Microsoft.AspNetCore.Mvc;

namespace Equilibrium.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("Paciente"))
                return RedirectToAction("Index", "Paciente");

            if (User.IsInRole("Medico"))
                return RedirectToAction("Index", "Medico");

            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Admin");
        }

        return RedirectToAction("Login", "Conta");
    }

    public IActionResult Error()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}