using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProjetFinal.Models;

namespace ProjetFinal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Page publique
        [AllowAnonymous]
        public IActionResult Index()
        {
            // (Optionnel) Si connecté, redirige vers une page utile
            // if (User?.Identity?.IsAuthenticated ?? false)
            //     return RedirectToAction("Index", "Projets");

            return View();
        }

        // Page publique
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
