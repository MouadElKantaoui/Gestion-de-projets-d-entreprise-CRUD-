using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetFinal.Data;
using ProjetFinal.Models;

namespace ProjetFinal.Controllers
{
    [Authorize]
    public class DeveloppeursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeveloppeursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------------
        // AUTH: Login / Logout
        // ---------------------------

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(int code, string motDePasse, string? returnUrl = null)
        {
            var dev = await _context.Developpeurs
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Code == code);

            if (dev == null || string.IsNullOrWhiteSpace(dev.MotDePasse) || dev.MotDePasse != motDePasse)
            {
                ModelState.AddModelError(string.Empty, "Code ou mot de passe invalide.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, dev.Id.ToString()),
                new Claim(ClaimTypes.Name, dev.Nom),
                new Claim(ClaimTypes.Role, dev.Anciennete)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => View();

        // ---------------------------
        // CRUD
        // ---------------------------

        public async Task<IActionResult> Index()
        {
            var devs = _context.Developpeurs
                .Include(d => d.Entreprise)
                .AsNoTracking();

            return View(await devs.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var developpeur = await _context.Developpeurs
                .Include(d => d.Entreprise)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (developpeur == null) return NotFound();

            return View(developpeur);
        }

        // --- Create (autorisé anonymement pour créer les 1ers comptes)
        [AllowAnonymous]
        public IActionResult Create()
        {
            ViewData["EntrepriseId"] = new SelectList(_context.Entreprises.AsNoTracking(), "Id", "Nom");
            ViewBag.AncienneteList = new SelectList(new[] { "Senior", "Junior" });
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,Code,MotDePasse,Anciennete,EntrepriseId")] Developpeur developpeur)
        {
            if (string.IsNullOrWhiteSpace(developpeur.Anciennete) ||
                !new[] { "Senior", "Junior" }.Contains(developpeur.Anciennete, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(Developpeur.Anciennete), "Ancienneté doit être 'Senior' ou 'Junior'.");
            }

            if (await _context.Developpeurs.AnyAsync(d => d.Code == developpeur.Code))
            {
                ModelState.AddModelError(nameof(Developpeur.Code), "Ce code est déjà utilisé par un autre développeur.");
            }

            if (ModelState.IsValid)
            {
                developpeur.Anciennete = NormalizeSeniorJunior(developpeur.Anciennete);
                _context.Add(developpeur);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EntrepriseId"] = new SelectList(_context.Entreprises.AsNoTracking(), "Id", "Nom", developpeur.EntrepriseId);
            ViewBag.AncienneteList = new SelectList(new[] { "Senior", "Junior" }, developpeur.Anciennete);
            return View(developpeur);
        }

        // --- Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var developpeur = await _context.Developpeurs.FindAsync(id);
            if (developpeur == null) return NotFound();

            ViewData["EntrepriseId"] = new SelectList(_context.Entreprises.AsNoTracking(), "Id", "Nom", developpeur.EntrepriseId);
            ViewBag.AncienneteList = new SelectList(new[] { "Senior", "Junior" }, developpeur.Anciennete);
            return View(developpeur);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Code,MotDePasse,Anciennete,EntrepriseId")] Developpeur developpeur)
        {
            if (id != developpeur.Id) return NotFound();

            if (string.IsNullOrWhiteSpace(developpeur.Anciennete) ||
                !new[] { "Senior", "Junior" }.Contains(developpeur.Anciennete, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(Developpeur.Anciennete), "Ancienneté doit être 'Senior' ou 'Junior'.");
            }

            if (await _context.Developpeurs.AnyAsync(d => d.Code == developpeur.Code && d.Id != developpeur.Id))
            {
                ModelState.AddModelError(nameof(Developpeur.Code), "Ce code est déjà utilisé par un autre développeur.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    developpeur.Anciennete = NormalizeSeniorJunior(developpeur.Anciennete);

                    _context.Update(developpeur);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeveloppeurExists(developpeur.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EntrepriseId"] = new SelectList(_context.Entreprises.AsNoTracking(), "Id", "Nom", developpeur.EntrepriseId);
            ViewBag.AncienneteList = new SelectList(new[] { "Senior", "Junior" }, developpeur.Anciennete);
            return View(developpeur);
        }

        // --- Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var developpeur = await _context.Developpeurs
                .Include(d => d.Entreprise)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (developpeur == null) return NotFound();

            return View(developpeur);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var developpeur = await _context.Developpeurs.FindAsync(id);
            if (developpeur != null)
            {
                _context.Developpeurs.Remove(developpeur);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DeveloppeurExists(int id) => _context.Developpeurs.Any(e => e.Id == id);

        private static string NormalizeSeniorJunior(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "Junior";
            var v = value.Trim().ToLowerInvariant();
            return v.StartsWith("sen") ? "Senior" : "Junior";
        }
    }
}
